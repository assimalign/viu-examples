// Setup: npm install --prefix _out/validation playwright
// Start both examples, then: node scripts/validate-browser.mjs [evidence-name]
import assert from 'node:assert/strict';
import { createRequire } from 'node:module';
import { mkdir, writeFile } from 'node:fs/promises';
import { existsSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import http from 'node:http';
import { gunzipSync, brotliDecompressSync } from 'node:zlib';

const require = createRequire(new URL('../_out/validation/package.json', import.meta.url));
const { chromium } = require('playwright');
const { expect } = require('playwright/test');
const label = process.argv[2] ?? 'debug';
const output = new URL(`../_out/validation/${label}/`, import.meta.url);
await mkdir(output, { recursive: true });
const executablePath = process.env.CHROME_PATH ??
    (existsSync('C:/Program Files/Google/Chrome/Application/chrome.exe')
        ? 'C:/Program Files/Google/Chrome/Application/chrome.exe' : undefined);
const browser = await chromium.launch({ headless: true, executablePath });
const evidence = { timestamp: new Date().toISOString(), browser: browser.version(), checks: [], diagnostics: [], api: [] };
let currentCheck = 'startup';
async function check(name, run) {
    currentCheck = name;
    try {
        const detail = await run();
        evidence.checks.push({ name, result: 'PASS', detail });
        console.log(`PASS ${name}${detail ? ': ' + JSON.stringify(detail) : ''}`);
    } catch (error) {
        evidence.checks.push({ name, result: 'FAIL', error: error.stack });
        console.error(`FAIL ${name}: ${error.message}`);
    }
}
async function open(port) {
    const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
    page.on('pageerror', error => evidence.diagnostics.push({ port, check: currentCheck, type: 'pageerror', text: error.stack }));
    page.on('console', message => {
        if (['warning', 'error'].includes(message.type()))
            evidence.diagnostics.push({ port, check: currentCheck, type: message.type(), text: message.text(), location: message.location() });
    });
    page.on('response', response => {
        if (response.status() >= 400)
            evidence.diagnostics.push({ port, check: currentCheck, type: 'http', url: response.url(), status: response.status() });
        if (response.url().endsWith('/api/tasks'))
            evidence.api.push({ port, method: response.request().method(), status: response.status(), body: response.request().postData() });
    });
    await page.goto(`http://127.0.0.1:${port}`);
    await expect(page.locator('.counter output')).toHaveText('0');
    await page.evaluate(() => { window.validationDocument = 'same-document'; });
    return page;
}
async function sameDocument(page) {
    assert.equal(await page.evaluate(() => window.validationDocument), 'same-document', 'form navigated or reloaded');
}
async function styles(page, port) {
    const computed = await page.locator('.card').first().evaluate(element => {
        const css = getComputedStyle(element);
        return { display: css.display, borderRadius: css.borderRadius, borderWidth: css.borderTopWidth };
    });
    assert.deepEqual(computed, { display: 'grid', borderRadius: '12px', borderWidth: '1px' });
    const path = '/_content/Assimalign.Viu.Showcase.Components/Assimalign.Viu.Showcase.Components.viu.css';
    const encodings = [];
    for (const requested of ['identity', 'gzip', 'br']) {
        const response = await new Promise((resolve, reject) => {
            http.get(`http://127.0.0.1:${port}${path}`, { headers: { 'Accept-Encoding': requested } }, res => {
                const chunks = [];
                res.on('data', chunk => chunks.push(chunk));
                res.on('end', () => resolve({ status: res.statusCode, headers: res.headers, bytes: Buffer.concat(chunks) }));
                res.on('error', reject);
            }).on('error', reject);
        });
        assert.equal(response.status, 200);
        assert.match(response.headers['content-type'], /text\/css/);
        const encoding = response.headers['content-encoding'];
        const decoded = encoding === 'gzip' ? gunzipSync(response.bytes)
            : encoding === 'br' ? brotliDecompressSync(response.bytes) : response.bytes;
        assert.match(decoded.toString(), /\.card\s*\{/);
        assert.doesNotMatch(decoded.toString(), /<Project/);
        encodings.push({ requested, received: encoding ?? 'identity', bytes: response.bytes.length, decodedBytes: decoded.length });
    }
    return { computed, encodings };
}

try {
    const standalone = await open(5180);
    await check('StandAlone counter typed event', async () => {
        await standalone.getByRole('button', { name: 'Increment', exact: true }).click();
        await expect(standalone.locator('.counter output')).toHaveText('2');
        await expect(standalone.getByText('last value 2', { exact: true })).toBeVisible();
        await standalone.getByRole('button', { name: 'Decrement', exact: true }).click();
        await expect(standalone.getByText('last value 0', { exact: true })).toBeVisible();
    });
    await check('StandAlone Toggle bool model, ARIA and slot v-if', async () => {
        const toggle = standalone.getByRole('switch');
        await expect(toggle).toHaveAttribute('aria-checked', 'true');
        await expect(standalone.locator('.progress__label')).toHaveText('0 / 3');
        await toggle.click();
        await expect(toggle).toHaveAttribute('aria-checked', 'false');
        await expect(standalone.getByRole('progressbar')).toHaveCount(0);
        await toggle.click();
        await expect(toggle).toHaveAttribute('aria-checked', 'true');
        await expect(standalone.getByRole('progressbar')).toHaveCount(1);
    });
    await check('StandAlone library styles and HTTP compression', () => styles(standalone, 5180));
    await standalone.getByRole('link', { name: /^Tasks/ }).click();
    const title = standalone.getByRole('textbox', { name: 'Task title' });
    await check('StandAlone Card slot typed model, key filter and Enter', async () => {
        await expect(standalone.locator('.card .task-form')).toHaveCount(1);
        await title.fill('  Browser Enter task  ');
        await expect(standalone.getByRole('button', { name: 'Add task', exact: true })).toBeEnabled();
        await title.press('ArrowLeft');
        await expect(standalone.locator('.task-list li')).toHaveCount(3);
        await title.press('Enter');
        await expect(standalone.locator('.task-list li')).toHaveCount(4);
        await expect(standalone.getByRole('link', { name: 'Browser Enter task', exact: true })).toBeVisible();
        await expect(title).toHaveValue('');
        await sameDocument(standalone);
    });
    await check('StandAlone form submit button without navigation', async () => {
        await title.fill('Browser submit task');
        await standalone.getByRole('button', { name: 'Add task', exact: true }).click();
        await expect(standalone.locator('.task-list li')).toHaveCount(5);
        await expect(title).toHaveValue('');
        await sameDocument(standalone);
    });
    await check('StandAlone checkbox model and reactive summary', async () => {
        await standalone.getByRole('checkbox', { name: 'Browser Enter task', exact: true }).check();
        await expect(standalone.locator('.task--done')).toHaveCount(1);
        await expect(standalone.getByText('4 of 5 open', { exact: true })).toBeVisible();
    });
    await check('StandAlone typed detail route and Toggle listener', async () => {
        await standalone.getByRole('link', { name: 'Browser Enter task', exact: true }).click();
        await expect(standalone).toHaveURL(/#\/tasks\/4$/);
        const toggle = standalone.getByRole('switch', { name: 'Completed', exact: true });
        await expect(toggle).toHaveAttribute('aria-checked', 'true');
        await toggle.click();
        await expect(toggle).toHaveAttribute('aria-checked', 'false');
        await expect(standalone.getByText('open', { exact: true })).toBeVisible();
        await standalone.getByRole('link', { name: /Back to tasks/ }).click();
        await expect(standalone.getByRole('checkbox', { name: 'Browser Enter task', exact: true })).not.toBeChecked();
    });
    await check('StandAlone Clear completed keyed list', async () => {
        await standalone.getByRole('checkbox', { name: 'Browser submit task', exact: true }).check();
        await standalone.getByRole('button', { name: 'Clear completed', exact: true }).click();
        await expect(standalone.locator('.task-list li')).toHaveCount(4);
        await expect(standalone.getByRole('link', { name: 'Browser submit task', exact: true })).toHaveCount(0);
        await sameDocument(standalone);
    });
    await standalone.screenshot({ path: fileURLToPath(new URL('standalone-tasks.png', output)), fullPage: true });
    await check('StandAlone missing task and not-found route', async () => {
        await standalone.goto('http://127.0.0.1:5180/#/tasks/999');
        await expect(standalone.getByRole('heading', { name: 'Task not found', exact: true })).toBeVisible();
        await standalone.goto('http://127.0.0.1:5180/#/does-not-exist');
        await expect(standalone.getByText('That page does not exist.', { exact: true })).toBeVisible();
    });

    const cohesion = await open(5181);
    await check('Cohesion DI greeting typed model inside Card slot', async () => {
        await expect(cohesion.locator('.card input')).toHaveCount(1);
        await cohesion.getByRole('textbox', { name: 'Your name', exact: true }).fill('Ada');
        await expect(cohesion.getByText(/^Hello Ada — composed by Cohesion DI/)).toBeVisible();
        return await cohesion.locator('.card__body p').first().innerText();
    });
    await check('Cohesion library styles and HTTP compression', () => styles(cohesion, 5181));
    await cohesion.screenshot({ path: fileURLToPath(new URL('cohesion-home.png', output)), fullPage: true });
    await cohesion.getByRole('link', { name: 'Tasks', exact: true }).click();
    await check('Cohesion Tasks GET and Enter POST', async () => {
        await expect(cohesion.locator('.task-list li')).toHaveCount(3);
        await cohesion.getByRole('textbox', { name: 'Task title' }).fill('  Cohesion Enter task  ');
        await cohesion.getByRole('textbox', { name: 'Task title' }).press('Enter');
        await expect(cohesion.locator('.task-list li')).toHaveCount(4);
        await expect(cohesion.getByText('Cohesion Enter task', { exact: true })).toBeVisible();
        await sameDocument(cohesion);
    });
    await check('Cohesion submit POST without navigation', async () => {
        await cohesion.getByRole('textbox', { name: 'Task title' }).fill('Cohesion submit task');
        await cohesion.getByRole('button', { name: 'Add task', exact: true }).click();
        await expect(cohesion.locator('.task-list li')).toHaveCount(5);
        await sameDocument(cohesion);
        assert.equal(evidence.api.filter(row => row.method === 'POST' && row.status === 201).length, 2);
    });
    await check('Cohesion /api/tasks JSON and /report SSR without scripts', async () => {
        const api = await cohesion.request.get('http://127.0.0.1:5181/api/tasks');
        assert.equal(api.status(), 200);
        assert.match(api.headers()['content-type'], /application\/json/);
        const tasks = await api.json();
        assert.equal(tasks.length, 5);
        assert.equal(tasks.filter(task => task.title === 'Cohesion Enter task').length, 1);
        const report = await cohesion.request.get('http://127.0.0.1:5181/report');
        const html = await report.text();
        await writeFile(new URL('report.html', output), html);
        assert.equal(report.status(), 200);
        assert.equal(report.headers()['cache-control'], 'no-store');
        assert.match(html, /Cohesion submit task/);
        assert.match(html, /2 \/ 5/);
        assert.doesNotMatch(html, /<script\b/i);
        return { tasks: tasks.length, reportStatus: report.status(), scripts: 0, compoundProgress: '2 / 5' };
    });
    await check('Cohesion /tasks direct deep link SPA fallback', async () => {
        const response = await cohesion.goto('http://127.0.0.1:5181/tasks');
        assert.equal(response.status(), 200);
        await expect(cohesion.getByRole('heading', { name: 'Tasks', exact: true })).toBeVisible();
        await expect(cohesion.locator('.task-list li')).toHaveCount(5);
    });
    await check('Cohesion report browser rendering', async () => {
        await cohesion.goto('http://127.0.0.1:5181/report');
        await expect(cohesion.getByText('Cohesion submit task', { exact: true })).toBeVisible();
        await expect(cohesion.locator('script')).toHaveCount(0);
        await expect(cohesion.locator('.progress__label')).toHaveText('2 / 5');
        await cohesion.screenshot({ path: fileURLToPath(new URL('cohesion-report.png', output)), fullPage: true });
    });
    await check('Cohesion not-found route', async () => {
        await cohesion.goto('http://127.0.0.1:5181/does-not-exist');
        await expect(cohesion.getByText('That page does not exist.', { exact: true })).toBeVisible();
    });
    await check('Framework console diagnostics', async () => {
        const framework = evidence.diagnostics.filter(item =>
            !(item.location?.url ?? item.url ?? '').endsWith('/favicon.ico'));
        assert.deepEqual(framework, [], 'Unexpected framework diagnostics; see results.json');
    });
} catch (error) {
    evidence.checks.push({ name: 'browser setup/navigation', result: 'FAIL', error: error.stack });
} finally {
    await writeFile(new URL('results.json', output), JSON.stringify(evidence, null, 2));
    await browser.close();
    console.log(JSON.stringify({ passed: evidence.checks.filter(c => c.result === 'PASS').length,
        failed: evidence.checks.filter(c => c.result === 'FAIL').length, diagnostics: evidence.diagnostics }, null, 2));
    if (evidence.checks.some(c => c.result === 'FAIL')) process.exitCode = 1;
}

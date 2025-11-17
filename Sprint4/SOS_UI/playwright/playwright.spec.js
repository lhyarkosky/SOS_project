import { test, expect } from '@playwright/test';

test('homepage loads and shows SOS Game title', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await expect(page.locator('text=SOS Game')).toBeVisible();
});

import { test, expect } from "@playwright/test";

var uri: string = "http://localhost:1080/";

test("Add Meme Test", async ({ page }) => {
  await page.goto(uri);

  const add = page.locator(".form-row button").first();

  await page.waitForSelector(".spinner-grow", { state: "detached" });

  const cards = page.locator(".card");

  const beforeCardCount = await cards.count();

  await Promise.all([
    page.waitForResponse(
      (response) =>
        response.url().endsWith("/image") && response.status() === 200
    ),
    await add.click(),
  ]);

  const afterCardCount = await cards.count();

  expect(afterCardCount).toEqual(beforeCardCount + 1);
});

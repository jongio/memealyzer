import { test, expect, WebSocket } from "@playwright/test";
import { TextMessageFormat } from "./textMessageFormat";

let uri = process.env.WEBAPP_ENDPOINT || "http://localhost:1080/";

interface Image {
  id: string;
  Id: string;
}

interface ImageEvent {
  arguments: Image[];
}

test("Add Meme Test", async ({ page }) => {
  // This test will add a meme and check SignalR response

  const [webSocket] = await Promise.all([
    page.waitForEvent("websocket"),
    page.goto(uri),
  ]);

  const add = page.locator(".form-row button").first();

  await page.waitForSelector(".spinner-grow", { state: "detached" });

  const cards = page.locator(".card");

  const beforeCardCount = await cards.count();

  let expectedId: string;
  let actualId: string;

  await Promise.all([
    page
      .waitForResponse(
        (response) =>
          response.url().endsWith("/image") && response.status() === 200
      )
      .then(async (response) => {
        const image = (await response.json()) as Image;
        expectedId = image.id;
      }),
    add.click(),
  ]);

  const afterCardCount = await cards.count();

  expect(afterCardCount).toEqual(beforeCardCount + 1);

  await webSocket.waitForEvent("framereceived", (event) => {
    if (event.payload.indexOf("ReceiveImage") > 0) {
      const imageEvents = TextMessageFormat.parse(event.payload.toString());
      for (let imageEventRaw of imageEvents) {
        const imageEvent = JSON.parse(imageEventRaw) as ImageEvent;
        actualId = imageEvent.arguments[0].Id;
        return true;
      }
    }
  });

  expect(actualId).toBe(expectedId);
});
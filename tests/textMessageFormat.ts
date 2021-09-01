export class TextMessageFormat {
    public static RecordSeparatorCode = 0x1e;
    public static RecordSeparator = String.fromCharCode(TextMessageFormat.RecordSeparatorCode);

    public static write(output: string): string {
        return `${output}${TextMessageFormat.RecordSeparator}`;
    }

    public static parse(input: string): string[] {
        if (input[input.length - 1] !== TextMessageFormat.RecordSeparator) {
            throw new Error("Message is incomplete.");
        }

        const messages = input.split(TextMessageFormat.RecordSeparator);
        messages.pop();
        return messages;
    }
}
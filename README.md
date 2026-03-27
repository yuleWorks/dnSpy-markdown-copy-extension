# dnSpy Markdown Copy Extension (for use with Obsidian)

Custom dnSpy extension for copying selected decompiled code to formatted, color-preserving Markdown, matching the default dnSpy dark theme.

## What it does

This extension adds the ability to copy document text with Markdown formatting that preserves the indentation and theme of the original text when pasted into Obsidian. 

## Dependency 

This extension must be used with a minimal dnSpy fork that exposes:

- `DocumentViewerContent.ColorCollection`

Fork: https://github.com/yuleWorks/dnSpy-exposed-ColorCollection/

## Build

This project is intended to be built within the dnSpy source tree, specifically from within `dnSpy/Extensions/`.

After building, copy the compiled extension DLL to a folder within the forked dnSpy extension directory:

`dnSpy/dnSpy/bin/Release/netX.Y/win-x64/Extensions/DnSpyMarkdownExtension/`

## Notes

This was intended to be a one-off tool rather than a general-purpose extension framework.

## Usage

Select text within dnSpy, right-click, and select "Copy with Markdown" to copy formatted text to the clipboard.

# Locale Atlas — every translation key of every mod

The Locale Atlas maps the localization of all installed mods: which keys each mod declares in each language, where the keys are used — scenarios, C# code, JSON data, collections, scenes, manifests — and what is wrong. It separates two kinds of problems:

- **errors** — broken text: a key that is used but declared nowhere, an empty value, one key with two different values in one mod, a file the loader will not read, a broken encoding;
- **warnings** — translation gaps: a key missing in some language. The game then falls back to a language that has the text, so nothing breaks; the warning says which line to add.

## Running it

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

It reads every installed mod, writes `report.md` and `report.json` into the `--locale-out` folder, logs a summary line and exits with 1 if there are errors. The test runner has it as the `locale_atlas` test. The text of `report.md` is written in Russian; the problem descriptions are in English.

A clean run of the current mods:

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## A gap and a break, side by side

To show both kinds, two changes were made to the novel template's mod for one run.

**A missing translation.** The Spanish line of the town's question was removed from `locale/scenarios_es.properties`. The atlas reports a warning with the line to add:

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

A Spanish player still sees a question — in another language — so this is a gap, not a breakage.

**A broken reference.** In the town's scenario the key of an answer was misspelt:

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

Now the key is used but declared in no file, and the atlas reports an **error** for each language, pointing at the scenario line:

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

The run ended with `7 error(s)` and failed. With the key spelt `opt_bar` again and the Spanish line back, it is clean.

## What the report contains

- **Encodings** — every locale file with its encoding, line endings and number of keys; problems such as non-UTF-8, U+FFFD, control characters, mojibake or CR-only line breaks are errors.
- **Keys that need attention** — one row per key: a mark per language (`✓` declared, `∅` empty, `—` missing), where it is declared, who refers to it, and what is wrong.
- **Problems** — errors and warnings with the mod, key, language, the place to fix and the exact line to add.

Other warnings: a BOM, mixed line endings, the same key twice with the same value, a mod overriding a key only in some languages, `{0}` placeholders that differ between languages, Cyrillic in English text, `???` in a value, a mod without its `mod_<id>_name` and `mod_<id>_desc`.

## Where keys come from

- `.scenario` lines with `key=`; translations of scenario text go to `locale/scenarios_<lang>.properties`;
- `Translate("…")`, `Tr("…")` and `Locale = "…"` in C#;
- text fields of JSON data (`Name`, `Description`, `title`…), `"@key"` in collections;
- auto-translated `text` and `tooltip_text` in `.tscn` scenes;
- the manifest's `mod_<id>_name` and `mod_<id>_desc`.

Locale files must be directly in a mod's `locale/` folder; a file in a subfolder is not read by the game and is an error.

## If something goes wrong

1. **An error for a key you see working in English.** The scenario line carries its own text, so the line may still read correctly in one language while the translations of the misspelt key are found nowhere.
2. **Hundreds of warnings.** They are translation gaps; each row gives the file and the line to add.

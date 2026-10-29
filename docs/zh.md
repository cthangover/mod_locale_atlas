# Locale Atlas — 每个模组的每一个翻译键

本地化图谱描绘所有已安装模组的本地化情况：每个模组在每种语言里声明了哪些键、这些键在哪里被使用——剧情、C# 代码、JSON 数据、合集、场景、清单——以及哪里出了问题。它把问题分成两类：

- **错误** — 文本损坏：一个键被使用却没有在任何地方声明；值为空；同一个键在同一个模组里有两个不同的值；加载器读不了的文件；损坏的编码；
- **警告** — 翻译缺口：某个键在某门语言里缺失。这时游戏会回退到有这段文本的语言，所以什么都不会坏；警告会说明该添加哪一行。

## 如何运行

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

它读取每个已安装的模组，把 `report.md` 和 `report.json` 写进 `--locale-out` 文件夹，在日志里输出一行汇总，并在有错误时以退出码 1 结束。测试运行器把它当作 `locale_atlas` 测试。`report.md` 的正文是俄语，问题描述是英语。

对当前各模组的一次干净运行：

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## 缺口与损坏，并排来看

为了展示这两类问题，在小说模板的模组里为一次运行做了两处改动。

**缺失的翻译。** 小镇问题那一行西班牙语从 `locale/scenarios_es.properties` 里被删掉了。图谱报告一条警告，并给出该添加的行：

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

西班牙语玩家仍然会看到一个提问——不过是另一种语言的——所以这是缺口，而不是损坏。

**损坏的引用。** 在小镇的剧情里，一个答案的键拼错了：

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

现在这个键被使用，却没有在任何文件里声明，图谱对每种语言各报告一条**错误**，并指向剧情里的那一行：

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

那次运行以 `7 error(s)` 结束并失败。把键重新拼成 `opt_bar`、把西班牙语那行加回来之后，一切又干净了。

## 报告里有什么

- **编码** — 每个本地化文件的编码、换行符和键数；非 UTF-8、U+FFFD、控制字符、乱码（mojibake）或仅用 CR 换行等问题都属于错误。
- **需要关注的键** — 每个键一行：每种语言一个标记（`✓` 已声明、`∅` 为空、`—` 缺失），在哪里声明、谁在引用它、哪里有问题。
- **问题** — 错误和警告，附模组、键、语言、要修改的位置以及要添加的准确一行。

其他警告：BOM、混杂的换行符、同一个键以相同的值出现两次、模组只在部分语言里覆盖某个键、各语言之间不一致的 `{0}` 占位符、英文文本里出现西里尔字母、值里出现 `???`，以及模组缺少自己的 `mod_<id>_name` 和 `mod_<id>_desc`。

## 键从哪里来

- 带 `key=` 的 `.scenario` 行；剧情文本的翻译进入 `locale/scenarios_<lang>.properties`；
- C# 里的 `Translate("…")`、`Tr("…")` 和 `Locale = "…"`；
- JSON 数据的文本字段（`Name`、`Description`、`title`……），合集里的 `"@key"`；
- `.tscn` 场景里自动翻译的 `text` 和 `tooltip_text`；
- 清单里的 `mod_<id>_name` 和 `mod_<id>_desc`。

本地化文件必须直接放在模组的 `locale/` 文件夹里；子文件夹里的文件游戏读不到，算一个错误。

## 如果出了问题

1. **某个键在英语里明明正常，却报了错误。** 剧情行自带文本，所以它在一种语言里可能仍然显示正常，而拼错的键的翻译却哪里都找不到。
2. **成百上千条警告。** 它们是翻译缺口；每一行都给出文件名和该添加的那一行。


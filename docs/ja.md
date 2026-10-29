# Locale Atlas — すべての MOD のすべての翻訳キー

Locale Atlas は、インストール済み全 MOD のローカライズを地図にします。どの MOD がどの言語でどのキーを宣言しているか、キーがどこで使われているか——シナリオ、C# コード、JSON データ、コレクション、シーン、マニフェスト——そして何がおかしいか。問題を 2 種類に分けます:

- **エラー** — 壊れたテキスト。使われているのにどこにも宣言されていないキー、空の値、同じ MOD 内で 2 つの異なる値を持つキー、ローダーが読めないファイル、壊れたエンコーディング;
- **警告** — 翻訳の欠け。ある言語で欠けているキー。この場合、ゲームはテキストがある言語へフォールバックするので、何も壊れません。警告には足すべき行が示されます。

## 実行方法

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

インストール済みの全 MOD を読み、`report.md` と `report.json` を `--locale-out` フォルダーに書き込み、サマリー行をログに出し、エラーがあれば終了コード 1 で終わります。テストランナーでは `locale_atlas` テストです。`report.md` の本文はロシア語、問題の説明は英語です。

現在の MOD のきれいな実行:

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## 欠けと壊れを並べて見る

2 種類を見せるために、小説テンプレートの MOD に 1 回の実行のための変更を 2 か所加えました。

**欠けている翻訳。** 町の質問のスペイン語行を `locale/scenarios_es.properties` から削除しました。アトラスは、足すべき行とともに警告を報告します:

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

スペイン語のプレイヤーにはそれでも質問が見えます——ただし別の言語で——。つまりこれは欠けであり、壊れではありません。

**壊れた参照。** 町のシナリオで、ある回答のキーのつづりを間違えました:

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

このキーは使われているのにどのファイルにも宣言されておらず、アトラスは言語ごとに**エラー**を報告し、シナリオの行を指します:

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

実行は `7 error(s)` で終わり、失敗しました。キーを再び `opt_bar` とつづり、スペイン語行を戻せば、またきれいになります。

## レポートの中身

- **エンコーディング** — ローカライズファイルごとのエンコーディング、改行、キー数。非 UTF-8、U+FFFD、制御文字、文字化け（mojibake）、CR のみの改行はエラーです。
- **注意が必要なキー** — キーごとに 1 行。言語ごとの印（`✓` 宣言済み、`∅` 空、`—` 欠落）、宣言場所、参照している箇所、そして何がおかしいか。
- **問題** — MOD・キー・言語・直す場所・足すべき正確な行つきの、エラーと警告。

その他の警告: BOM、混在する改行、同じ値で 2 回現れる同じキー、一部の言語だけでキーを上書きする MOD、言語間で異なる `{0}` プレースホルダー、英語テキスト内のキリル文字、値の中の `???`、`mod_<id>_name` と `mod_<id>_desc` のない MOD。

## キーはどこから来るか

- `key=` の付いた `.scenario` 行。シナリオテキストの翻訳は `locale/scenarios_<lang>.properties` へ;
- C# の `Translate("…")`、`Tr("…")`、`Locale = "…"`;
- JSON データのテキスト欄（`Name`、`Description`、`title` など）、コレクションの `"@key"`;
- `.tscn` シーンで自動翻訳される `text` と `tooltip_text`;
- マニフェストの `mod_<id>_name` と `mod_<id>_desc`。

ローカライズファイルは MOD の `locale/` フォルダー直下になければなりません。サブフォルダー内のファイルはゲームに読まれず、エラーです。

## 何かがおかしいとき

1. **英語では正しく見えるキーにエラー。** シナリオ行は自分のテキストを運ぶので、つづりを間違えたキーの翻訳がどこにも見つからないまま、ある言語では正しく読めることがあります。
2. **何百もの警告。** これらは翻訳の欠けです。各行が、足すべきファイルと行を示します。


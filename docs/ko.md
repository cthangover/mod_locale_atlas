# Locale Atlas — 모든 모드의 모든 번역 키

Locale Atlas는 설치된 모든 모드의 현지화를 지도로 만듭니다. 각 모드가 각 언어로 어떤 키를 선언하는지, 키가 어디서 쓰이는지—시나리오, C# 코드, JSON 데이터, 컬렉션, 씬, 매니페스트—그리고 무엇이 잘못되었는지 보여 줍니다. 문제를 두 종류로 나눕니다:

- **오류** — 깨진 텍스트. 쓰이지만 어디에도 선언되지 않은 키, 빈 값, 한 모드 안에서 두 개의 다른 값을 가진 키, 로더가 읽지 못할 파일, 깨진 인코딩;
- **경고** — 번역 공백. 어떤 언어에서 빠진 키. 이때 게임은 텍스트가 있는 언어로 되돌아가므로 아무것도 깨지지 않고, 경고는 채워 넣을 줄을 알려 줍니다.

## 실행 방법

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

설치된 모든 모드를 읽고, `report.md`와 `report.json`을 `--locale-out` 폴더에 쓰고, 요약 줄을 로그에 남기며, 오류가 있으면 종료 코드 1로 끝납니다. 테스트 러너에서는 `locale_atlas` 테스트입니다. `report.md`의 본문은 러시아어이고, 문제 설명은 영어입니다.

현재 모드들의 깨끗한 실행:

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## 공백과 고장, 나란히 놓고 보기

두 종류를 보여 드리기 위해, 한 번의 실행을 위해 소설 템플릿의 모드에 두 가지 변경을 했습니다.

**빠진 번역.** 마을 질문의 스페인어 줄을 `locale/scenarios_es.properties`에서 지웠습니다. 아틀라스는 채워 넣을 줄과 함께 경고를 보고합니다:

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

스페인어 플레이어에게도 여전히 질문이 보입니다—다른 언어로—그러니 이것은 고장이 아니라 공백입니다.

**깨진 참조.** 마을 시나리오에서 한 답변의 키 철자를 틀렸습니다:

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

이제 이 키는 쓰이지만 어느 파일에도 선언되지 않았고, 아틀라스는 언어마다 하나씩 **오류**를 보고하며 시나리오의 그 줄을 가리킵니다:

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

실행은 `7 error(s)`로 끝났고 실패했습니다. 키를 다시 `opt_bar`로 쓰고 스페인어 줄을 돌려 놓으면 다시 깨끗해집니다.

## 리포트에 든 것

- **인코딩** — 로컬라이즈 파일마다 인코딩, 줄바꿈, 키 수. 비(非) UTF-8, U+FFFD, 제어 문자, mojibake, CR 전용 줄바꿈 같은 문제는 오류입니다.
- **주의가 필요한 키** — 키마다 한 행. 언어별 표식(`✓` 선언됨, `∅` 비었음, `—` 없음), 선언 위치, 참조하는 곳, 그리고 무엇이 잘못되었는지.
- **문제** — 모드·키·언어·고칠 위치·채워 넣을 정확한 줄이 붙은 오류와 경고.

그 밖의 경고: BOM, 뒤섞인 줄바꿈, 같은 값으로 두 번 나오는 같은 키, 일부 언어에서만 키를 재정의하는 모드, 언어 사이가 다른 `{0}` 자리표시자, 영어 텍스트 속 키릴 문자, 값 속의 `???`, `mod_<id>_name`과 `mod_<id>_desc`가 없는 모드.

## 키는 어디서 오는가

- `key=`가 붙은 `.scenario` 줄. 시나리오 텍스트의 번역은 `locale/scenarios_<lang>.properties`로 갑니다;
- C#의 `Translate("…")`, `Tr("…")`, `Locale = "…"`;
- JSON 데이터의 텍스트 필드(`Name`, `Description`, `title`…), 컬렉션의 `"@key"`;
- `.tscn` 씬에서 자동 번역되는 `text`와 `tooltip_text`;
- 매니페스트의 `mod_<id>_name`과 `mod_<id>_desc`.

로컬라이즈 파일은 모드의 `locale/` 폴더 바로 아래에 있어야 합니다. 하위 폴더의 파일은 게임이 읽지 않으며 오류입니다.

## 무언가 잘못되었을 때

1. **영어에서는 멀쩡히 보이는 키에 오류.** 시나리오 줄은 자기 텍스트를 직접 담고 있으므로, 철자가 틀린 키의 번역이 어디에도 없는데도 한 언어에서는 올바르게 읽힐 수 있습니다.
2. **수백 개의 경고.** 번역 공백입니다. 각 행이 채워 넣을 파일과 줄을 알려 줍니다.


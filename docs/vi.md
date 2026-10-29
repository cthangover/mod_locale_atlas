# Locale Atlas — mọi khóa dịch của mọi mod

Locale Atlas vẽ ra phần bản địa hóa của mọi mod đã cài: mỗi mod khai báo những khóa nào ở mỗi ngôn ngữ, các khóa được dùng ở đâu — kịch bản, mã C#, dữ liệu JSON, bộ sưu tập, cảnh, manifest — và cái gì không ổn. Nó tách thành hai loại vấn đề:

- **lỗi** — văn bản hỏng: một khóa được dùng nhưng không khai báo ở đâu cả, một giá trị rỗng, một khóa có hai giá trị khác nhau trong cùng một mod, một tệp mà bộ nạp không đọc được, mã hóa hỏng;
- **cảnh báo** — chỗ thiếu bản dịch: một khóa còn thiếu ở ngôn ngữ nào đó. Khi đó game sẽ quay về một ngôn ngữ có sẵn đoạn chữ, nên chẳng có gì vỡ; cảnh báo cho biết cần thêm dòng nào.

## Cách chạy

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

Nó đọc mọi mod đã cài, ghi `report.md` và `report.json` vào thư mục `--locale-out`, ghi một dòng tổng kết vào log và thoát với mã 1 nếu có lỗi. Trong test runner, nó là phép kiểm thử `locale_atlas`. Phần chữ của `report.md` viết bằng tiếng Nga; phần mô tả vấn đề viết bằng tiếng Anh.

Một lần chạy sạch với các mod hiện có:

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## Một chỗ thiếu và một chỗ hỏng, đặt cạnh nhau

Để thấy cả hai loại, người ta sửa hai chỗ trong mod của bản mẫu tiểu thuyết cho đúng một lần chạy.

**Một bản dịch bị thiếu.** Dòng tiếng Tây Ban Nha của câu hỏi thị trấn đã bị xóa khỏi `locale/scenarios_es.properties`. Atlas báo một cảnh báo kèm dòng cần thêm:

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

Người chơi tiếng Tây Ban Nha vẫn thấy một câu hỏi — bằng một ngôn ngữ khác — nên đây là chỗ thiếu, không phải chỗ hỏng.

**Một tham chiếu bị hỏng.** Trong kịch bản của thị trấn, khóa của một câu trả lời bị gõ nhầm chính tả:

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

Giờ khóa này được dùng nhưng không được khai báo trong tệp nào, và Atlas báo một **lỗi** cho mỗi ngôn ngữ, chỉ thẳng vào dòng kịch bản:

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

Lần chạy ấy kết thúc với `7 error(s)` và thất bại. Gõ lại khóa là `opt_bar` và trả lại dòng tiếng Tây Ban Nha thì mọi thứ sạch sẽ trở lại.

## Báo cáo chứa gì

- **Mã hóa** — mỗi tệp bản địa hóa kèm mã hóa, kiểu xuống dòng và số khóa; các vấn đề như không phải UTF-8, U+FFFD, ký tự điều khiển, mojibake hoặc xuống dòng chỉ có CR đều là lỗi.
- **Các khóa cần chú ý** — mỗi khóa một dòng: một dấu cho mỗi ngôn ngữ (`✓` đã khai báo, `∅` rỗng, `—` thiếu), khai báo ở đâu, ai tham chiếu đến nó, và cái gì không ổn.
- **Vấn đề** — lỗi và cảnh báo kèm mod, khóa, ngôn ngữ, chỗ cần sửa và dòng chính xác cần thêm.

Các cảnh báo khác: BOM, kiểu xuống dòng trộn lẫn, cùng một khóa xuất hiện hai lần với cùng một giá trị, một mod chỉ ghi đè khóa ở một số ngôn ngữ, trình giữ chỗ `{0}` không khớp giữa các ngôn ngữ, chữ Cyrillic trong văn bản tiếng Anh, `???` trong một giá trị, một mod thiếu `mod_<id>_name` và `mod_<id>_desc` của nó.

## Khóa đến từ đâu

- các dòng `.scenario` có `key=`; bản dịch của văn bản kịch bản đi vào `locale/scenarios_<lang>.properties`;
- `Translate("…")`, `Tr("…")` và `Locale = "…"` trong C#;
- các trường văn bản của dữ liệu JSON (`Name`, `Description`, `title`…), `"@key"` trong bộ sưu tập;
- `text` và `tooltip_text` được tự động dịch trong các cảnh `.tscn`;
- `mod_<id>_name` và `mod_<id>_desc` của manifest.

Tệp bản địa hóa phải nằm ngay trong thư mục `locale/` của mod; tệp trong thư mục con không được game đọc và được tính là lỗi.

## Nếu có gì đó không ổn

1. **Một lỗi cho khóa mà bạn thấy chạy tốt trong tiếng Anh.** Dòng kịch bản tự mang văn bản của nó, nên nó vẫn có thể đọc đúng ở một ngôn ngữ trong khi bản dịch của khóa gõ nhầm không tìm thấy ở đâu cả.
2. **Hàng trăm cảnh báo.** Đó là những chỗ thiếu bản dịch; mỗi dòng đều cho biết tệp và dòng cần thêm.


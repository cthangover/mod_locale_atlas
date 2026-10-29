# Locale Atlas — cada clave de traducción de cada mod

El Atlas de localización traza la localización de todos los mods instalados: qué claves declara cada mod en cada idioma, dónde se usan las claves —escenarios, código C#, datos JSON, colecciones, escenas, manifiestos— y qué está mal. Separa dos clases de problemas:

- **errores** — texto roto: una clave que se usa pero no se declara en ningún sitio, un valor vacío, una clave con dos valores distintos en un mismo mod, un archivo que el cargador no leerá, una codificación dañada;
- **advertencias** — huecos de traducción: una clave que falta en algún idioma. El juego entonces recurre a un idioma que tiene el texto, así que nada se rompe; la advertencia dice qué línea añadir.

## Cómo se ejecuta

```text
godot --path . -- --locale-atlas --locale-out=res://tests/artifacts/locale_atlas --with-test-suites
```

Lee todos los mods instalados, escribe `report.md` y `report.json` en la carpeta `--locale-out`, registra una línea de resumen y sale con 1 si hay errores. El test runner lo tiene como la prueba `locale_atlas`. El texto de `report.md` está escrito en ruso; las descripciones de los problemas, en inglés.

Una ejecución limpia de los mods actuales:

```text
locale atlas: 2284 key(s), 156 need attention, 0 error(s), 801 warning(s)
```

## Un hueco y una rotura, lado a lado

Para mostrar las dos clases, se hicieron dos cambios en el mod de la plantilla de novela durante una ejecución.

**Una traducción que falta.** Se quitó de `locale/scenarios_es.properties` la línea en español de la pregunta del pueblo. El atlas informa de una advertencia con la línea que hay que añadir:

```text
| warning | `my_novel_game` | `my_novel/town/prompt` | es | `locale/scenarios_en.properties:2` | declared in en, ja, ko, ru, vi, zh but not in 'es' | `locale/scenarios_es.properties: my_novel/town/prompt = Where to go?` |
```

Un jugador español sigue viendo una pregunta —en otro idioma—, así que esto es un hueco, no una rotura.

**Una referencia rota.** En el escenario del pueblo se equivocó la grafía de la clave de una respuesta:

```scenario
option "Go into the bar" key=my_novel/town/opt_baar -> :bar
```

Ahora la clave se usa pero no se declara en ningún archivo, y el atlas informa de un **error** por cada idioma, señalando la línea del escenario:

```text
| error | `my_novel_game` | `my_novel/town/opt_baar` | ru | `scenarios/my_novel_town.scenario:7` | referenced (scenario, 1×) but declared in no locale file | `locale/scenarios_ru.properties: my_novel/town/opt_baar = …` |
```

La ejecución terminó con `7 error(s)` y falló. Con la clave escrita de nuevo como `opt_bar` y la línea en español de vuelta, queda limpio.

## Qué contiene el informe

- **Codificaciones** — cada archivo de localización con su codificación, fines de línea y número de claves; problemas como no-UTF-8, U+FFFD, caracteres de control, mojibake o fines de línea solo con CR son errores.
- **Claves que requieren atención** — una fila por clave: una marca por idioma (`✓` declarada, `∅` vacía, `—` ausente), dónde se declara, quién la referencia y qué está mal.
- **Problemas** — errores y advertencias con el mod, la clave, el idioma, el sitio que corregir y la línea exacta que añadir.

Otras advertencias: un BOM, fines de línea mezclados, la misma clave dos veces con el mismo valor, un mod que sustituye una clave solo en algunos idiomas, marcadores `{0}` que difieren entre idiomas, cirílico en texto inglés, `???` en un valor, un mod sin su `mod_<id>_name` y `mod_<id>_desc`.

## De dónde salen las claves

- líneas `.scenario` con `key=`; las traducciones del texto del escenario van a `locale/scenarios_<lang>.properties`;
- `Translate("…")`, `Tr("…")` y `Locale = "…"` en C#;
- campos de texto de los datos JSON (`Name`, `Description`, `title`…), `"@key"` en colecciones;
- `text` y `tooltip_text` autotraducidos en las escenas `.tscn`;
- el `mod_<id>_name` y el `mod_<id>_desc` del manifiesto.

Los archivos de localización deben estar directamente en la carpeta `locale/` del mod; un archivo en una subcarpeta no lo lee el juego y es un error.

## Si algo va mal

1. **Un error para una clave que ves funcionar en inglés.** La línea del escenario lleva su propio texto, así que puede leerse correctamente en un idioma mientras las traducciones de la clave mal escrita no aparecen en ningún sitio.
2. **Cientos de advertencias.** Son huecos de traducción; cada fila da el archivo y la línea que añadir.


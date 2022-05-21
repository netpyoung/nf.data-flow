
엑셀과 템플릿을 이용해서 소스 파일을 만들고
그 소스파일을 컴파일한 dll을 추출
엑셀과 dll을 이용하여 db를 추출

소스를 그대로 쓰지않고 dll을 만든이유는
 혹여나 테이블과 매칭되지 않게 소스를 고치는 상황발생 방지
 

허나 dll로 되면서 버전컨트롤 관리시 테이블 소스의 변경 트레킹이 어려워짐


http://dotliquidmarkup.org/
https://shopify.github.io/liquid/
Liquid
Safe, customer-facing template language for flexible web apps.



$ 뒤에 오는 문자열의 시트(테이블)의 키를 참조합니다. (연결된 값이 해당 테이블에 없으면 오류)
~ 보조 인덱스를 배치하여 동일한 인덱스를 사용하는 테이블 내에서 중복 여부를 확인하는 기능입니다.



in 엑셀 폴더, in 템플릿 폴더, out 소스디렉토리
in 엑셀 폴더, out 디비디렉토리

| sheetname |                  |
| --------- | ---------------- |
| !         | prefix Const     |
| @         | prefix Enum      |
| #         | prefix Ignore    |
| ^         | postfix Inherint |
| #         | postfix Ignore   |

| reserved |                 | 필수여부 | 설명                         |
| -------- | --------------- | -------- | ---------------------------- |
| &END     | End of contents | O        | 첫 행, 첫 열에 존재          |
| TABLE    | 테이블명        | .        | 생략가능/ 없을시 시트명 사용 |
| TATTR    | 태이블 속성     | .        | 생략가능                     |
| TDESC    | 테이블 셜명     | .        | 생략가능                     |
| PART     | 파트            | .        | Client / Server / Common     |
| TYPE     | 타입            | O        |                              |
| NAME     | 이름            | O        |                              |
| VALUE    | 값              | O        |                              |
| DESC     | 셜명            | .        |                              |
| ATTR     | 속성            | .        |                              |


``` txt
// table
| TABLE | Hello   |     | &END |
| ----- | ------- | --- | ---- |
| TATTR | [Hello] |     |      |
| PART  | .       | ... |      |
| ATTR  | .       | ... |      |
| TYPE  | .       | ... |      |
| NAME  | .       | ... |      |
| DESC  | .       | ... |      |
| VALUE | .       | ... |      |
|       | .       | ... |      |
| &END  |         |     |      |

// !const
| TABLE    | Hello   |      |      |       |      | &END |
| -------- | ------- | ---- | ---- | ----- | ---- | ---- |
| TATTR    | [Hello] |      |      |       |      |      |
| PART     | ATTR    | TYPE | NAME | VALUE | DESC |      |
| .        | .       | .    | .    | .     | .    | .    |
| ...      | ...     | ...  | ...  | ...   | ...  | ...  |
| _ ignore |         |      |      |       |      |      |
| &END     |         |      |      |       |      |      |

// @enum
| TABLE    | Hello   |       |      |     | &END |
| -------- | ------- | ----- | ---- | --- | ---- |
| TATTR    | [Hello] |       |      |     |      |
| ATTR     | NAME    | VALUE | DESC |     |
| .        | .       | .     | .    | .   | .    |
| ...      | ...     | ...   | ...  | ... | ...  |
| _ ignore |         |       |      |     |      |
| &END     |         |       |      |     |      |
```

```

TATTR	[Hello]				&END
TABLE	Hello				
NAME	VALUE	DESC	PART	ATTR	
n1	1	할로우	Common		
n2	2	helelo	Common		
n3	3	ASDF	Client		
n4	4	ASDF	Server		
n5	5	ASDF	Common		
n6	6	ASDF	Common		
&END					
```

``` liquid
data.sheet_info
data.contents

data.sheet_info.sheet
data.sheet_info.sheet_name
data.sheet_info.type
data.sheet_info.row_max
data.sheet_info.column_max


// const.liquid
data.contents[0].part
data.contents[0].attr
data.contents[0].type
data.contents[0].name
data.contents[0].value
data.contents[0].desc

// enum.liquid
data.contents[0].attr
data.contents[0].name
data.contents[0].value
data.contents[0].desc

// class.liquid
data.contents[0].part
data.contents[0].attr
data.contents[0].type
data.contents[0].name
data.contents[0].desc
```
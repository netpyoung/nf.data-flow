
``` liquid
data.SheetInfo.SheetName
data.SheetInfo.SheetNamespace
data.SheetInfo.Type           // CONST | ENUM | CLASS
data.SheetInfo.RowMax
data.SheetInfo.ColumnMax


// const.liquid
data.Contents[0].Part
data.Contents[0].Attr
data.Contents[0].Type
data.Contents[0].Name
data.Contents[0].Value
data.Contents[0].Desc
/// TODO(pyoung): const.liquid에 {%- if content.Type == "string" -%} 로 처리하고 있는데... raw인지 ""감싸는지 구분할 무언가가 필요할듯.

// enum.liquid
data.Contents[0].Attr
data.Contents[0].Name
data.Contents[0].Value
data.Contents[0].Desc

// class.liquid
data.Contents[0].Part
data.Contents[0].Attr
data.Contents[0].Type
data.Contents[0].Name
data.Contents[0].Desc
```
# nf.data-flow

![GitHub Logo](flow.png)
[flow.puml](flow.puml)

## introduce
this is sample dataflow. When I making a game(in unity), I don't like to hard-coding for data class from excel. I'm writing custom class generator, and data exporter for that work with https://github.com/netpyoung/SqlCipher4Unity3D.

If you want to use [ScriptableObject](https://docs.unity3d.com/ScriptReference/ScriptableObject.html), check [Unity-QuickSheet](https://github.com/kimsama/Unity-QuickSheet).

## basic Knowledge.
* [Rakefile](https://github.com/ruby/rake)
* [dotnet cli](https://www.microsoft.com/net/core)
* [Sqlite](https://www.sqlite.org/) && [Sqlcipher](https://www.zetetic.net/sqlcipher/)

## dataflow
- support Enum, Const

### excel

* Sheet Name represent ClassName
    1. category: Common | Client | Server
    2. attribute
    3. type
    4. field name

`SampleCharacter`

| Common | Common       | Common | Common   |
| ------ | ------------ | ------ | -------- |
| Unique |              |        | Obsolete |
| int    | int          | string | int      |
| id     | character_id | name   | level    |
| 1      | 1            |  john  | 1        |
| 2      | 2            |  yoshi | 2        |
| 3      | 3            |  mario | 3        |
| 4      | 4            |  piona | 4        |


### unity

class.autogen.cs

```cs
    [Export]
    public class SampleCharacter
    {
        
        [Unique]
        public int id { get; set; }
        public int character_id { get; set; }
        public string name { get; set; }
        [Obsolete]
        public int level { get; set; }
    }
```

main

``` cs
void Start()
{
    DataService ds = new DataService("Assets/output/DB.db", "helloworld");
    List<SampleCharacter> characters = ds.Gets<SampleCharacter>();
    foreach (SampleCharacter c in characters)
    {
        Debug.Log($"{c.id} | {c.character_id} | {c.level} | {c.name}");
    }
}
```

## example

``` cmd
$ rake --version
rake, version 10.4.2

$ dotnet --version
2.2.203

$ rake
...

$ tree __BUILD
__BUILD
├── AutoGenerated.DB.dll
├── AutoGenerated.DB.pdb
├── DB.db
├── SQLite.Attribute.dll
└── SQLite.Attribute.pdb
```

## TODO

* [ ] serializable contructor for custom value.
* [ ] documentation

## Ref

* dotliquid:
* t4 : https://msdn.microsoft.com/en-us/library/bb126445.aspx
* scripty:  https://daveaglick.com/posts/announcing-scripty
* roslyn:  source generator

## Etc

* https://github.com/sqlitebrowser/sqlitebrowser

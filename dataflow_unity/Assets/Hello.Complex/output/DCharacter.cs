using System;
using SQLite.Attributes;
using System.ComponentModel;

namespace AutoGenerated.ComplexOne
{
    [Export]
    public class DCharacter
    {
        [PrimaryKey]
        [Description("아이디")]
        public int id { get; set; }
        [Unique]
        [Description("캐릭터아이디")]
        public int character_id { get; set; }
        
        [Description("이름")]
        public string name { get; set; }
        
        [Description("설명")]
        public string desc { get; set; }
        
        [Description("이넘")]
        public E_ENUM ee { get; set; }
        
        [Description("스타")]
        public bool isSpecial { get; set; }
        
        [Description("")]
        public DateTime xDT { get; set; }
        
        [Description("")]
        public DateTimeOffset xDateTimeOffset { get; set; }
        
        [Description("")]
        public TimeSpan xTimeSpan { get; set; }
        
        [Description("레벨")]
        public int level { get; set; }
    }
}
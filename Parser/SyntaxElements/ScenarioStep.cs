using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlInclude(typeof(Given))]
    [XmlInclude(typeof(When))]
    [XmlInclude(typeof(Then))]
    [XmlInclude(typeof(And))]
    [XmlInclude(typeof(But))]
    public class ScenarioStep
    {
        public string Text { get; set; }
        public string MultiLineTextArgument { get; set; }
        public FilePosition FilePosition { get; set; }

        private Dictionary<int, string[]> _tableArgLines = new Dictionary<int, string[]>();

        public ScenarioStep()
        {
        }

        public ScenarioStep(Text stepText, MultilineText multilineTextArgument, Table tableArg) // TODO: 20100428 jb: remove tableArg!
        {
            this.Text = stepText.Value;
            MultiLineTextArgument = multilineTextArgument == null ? null : multilineTextArgument.Value;
            this.TableArg = tableArg;
        }

        public Table TableArg
        {
            get
            {
                if(_tableArgLines.Count == 0)
                    return null;

                return new Table
                           {
                               Header = new Row{
                                                Cells = _tableArgLines.Values.First().Select(c => new Cell(new Text(c))).ToArray(),
                                                FilePosition = new FilePosition(_tableArgLines.Keys.First(), 0)
                                                },
                               Body = _tableArgLines.Skip(1).Select(r => new Row{
                                                                                Cells = r.Value.Select(c => new Cell(new Text(c))).ToArray(),
                                                                                FilePosition = new FilePosition(r.Key,0)
                                                                                }).ToArray()
                           };
            }
            set
            {
                // TODO: 20100428 jb: Setter is probably not needed
                _tableArgLines.Clear();
                if (value != null)
                {
                    ((ICollection<KeyValuePair<int, string[]>>)_tableArgLines).Add(new KeyValuePair<int,string[]>(value.Header.FilePosition.Line, value.Header.Cells.Select(c => c.Value).ToArray()));
                    foreach (var row in value.Body)
                    {
                        ((ICollection<KeyValuePair<int, string[]>>)_tableArgLines).Add(new KeyValuePair<int, string[]>(row.FilePosition.Line, row.Cells.Select(c => c.Value).ToArray()));
                    }
                }
            }
        }

        public void AddTableArgRow(string[] row, int linePosition)
        {
            _tableArgLines.Add(linePosition, row);
        }
    }

    public class ScenarioSteps : List<ScenarioStep>
    {
        public ScenarioSteps()
        {
        }

        public ScenarioSteps(params ScenarioStep[] thens)
        {
            AddRange(thens);
        }
    }
}
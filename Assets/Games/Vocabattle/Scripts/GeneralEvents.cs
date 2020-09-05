using System.Collections.Generic;

namespace Game.General
{
    public class WordsLoadedEvent : IEvent
    {
         private Dictionary<string, object> words;
         public WordsLoadedEvent(Dictionary<string, object> words)
         {
             this.words = words;
         }
         public object GetData()
         {
             return words;
         }
    }
}
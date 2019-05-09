using System;

namespace TechTalk.SpecFlow.Utils
{
    public interface IClock
    {
        DateTime GetToday();

        DateTime GetNowDateAndTime();
    }
}

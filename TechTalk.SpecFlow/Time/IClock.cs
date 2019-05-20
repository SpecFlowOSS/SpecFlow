using System;

namespace TechTalk.SpecFlow.Time
{
    public interface IClock
    {
        DateTime GetToday();

        DateTime GetNowDateAndTime();
    }
}

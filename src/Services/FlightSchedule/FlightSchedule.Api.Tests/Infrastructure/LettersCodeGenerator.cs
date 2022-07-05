using System;
using System.Linq;

namespace FlightSchedule.Api.Tests.Infrastructure;

public class LettersCodeGenerator
{
    private readonly char[] _currentCode;

    public LettersCodeGenerator(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "Should be more 0");
        }
        _currentCode = Enumerable.Repeat('A', length).ToArray();

    }

    public string Next()
    {
        void Increment(int index)
        {
            if (_currentCode[index] != 'Z')
            {
                _currentCode[index]++;
            }
            else if (index > 0)
            {
                Increment(index - 1);
            }
            else
            {
                _currentCode[0] = 'A';
            }
            for (var i = _currentCode.Length - 1; i > index; i--)
            {
                _currentCode[i] = 'A';
            }
        }
        lock (_currentCode)
        {
            Increment(2);
            return new string(_currentCode);
        }
    }

}
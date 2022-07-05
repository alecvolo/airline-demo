using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.Tests.Infrastructure;

public class LettersCodeGeneratorTests
{

    [Fact()]
    public void Letters_Codes_Generator_Should_Work()
    {
        var codeGenerator = new LettersCodeGenerator(3);
        char[] codes = { 'A', 'A', 'A' };
        for (var i = 0; i < 27 * 27 * 27 * 2; i++)
        {
            if (codes[2] != 'Z')
            {
                codes[2]++;
            }
            else if (codes[1] != 'Z')
            {
                codes[2] = 'A';
                codes[1]++;
            }
            else if (codes[0] != 'Z')
            {
                codes[2] = 'A';
                codes[1] = 'A';
                codes[0]++;
            }
            else
            {
                codes[2] = 'A';
                codes[1] = 'A';
                codes[0] = 'A';
            }

            new string(codes).Should().Be(codeGenerator.Next());

        }

    }
}
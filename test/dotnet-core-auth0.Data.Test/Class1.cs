namespace dotnet_core_auth0.Data.Test
{
  using FluentAssertions;
  using Xunit;

  public class Class1
  {

    [Fact]
    public void ShouldSucceed()
    {
      true.ShouldBeEquivalentTo(true);
    }

    [Fact]
    public void ShouldFail()
    {
      true.ShouldBeEquivalentTo(false, ".. reasons");
    }    
  }
}
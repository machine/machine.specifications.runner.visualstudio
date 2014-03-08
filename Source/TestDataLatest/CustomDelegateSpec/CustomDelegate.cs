using Machine.Specifications;

namespace CustomDelegateSpec
{
    [AssertDelegate]
    public delegate void They();

    [ActDelegate]
    public delegate void WhenDoing();
}
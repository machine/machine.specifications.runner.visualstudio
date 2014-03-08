using Machine.Specifications;

namespace CustomDelegateSpec
{
    [DelegateUsage(DelegateUsage.Assert)]
    public delegate void They();

    [DelegateUsage(DelegateUsage.Act)]
    public delegate void WhenDoing();
}
namespace MvcCoreTest.Utils
{
    using System;

    public interface ICaptchaGenerator
    {
        Tuple<byte[], string> Generate();
    }
}
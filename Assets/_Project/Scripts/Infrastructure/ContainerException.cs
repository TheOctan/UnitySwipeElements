using System;

namespace OctanGames.Infrastructure
{
    public class ContainerException : Exception
    {
        public ContainerException(string message) : base(message)
        {
        }
    }
}
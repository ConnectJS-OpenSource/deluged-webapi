﻿public class DelugeWebClientException : Exception
{
    public int Code { get; set; }

    public DelugeWebClientException(String message, int code) : base(message)
    {
    }
}
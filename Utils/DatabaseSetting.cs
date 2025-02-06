﻿namespace me.admin.api.Utils;

public class DatabaseSettings
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
}

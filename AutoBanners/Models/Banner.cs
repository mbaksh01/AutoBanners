namespace AutoBanners.Models;

public class Banner
{
    public string Message { get; set; } = "";

    public Priority Priority { get; set; } = Priority.p0;

    public Level Level { get; set; } = Level.Info;
}

public enum Level
{
    Info,
    Warning,
    Error
}

public enum Priority
{
    p0,
    p1,
    p2
}
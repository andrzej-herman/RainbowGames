using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RainbowSnake.Helpers;

public static class Images
{
	public static readonly ImageSource Empty = LoadImage("Empty.png");
	public static readonly ImageSource Food = LoadImage("Food.png");
	public static readonly ImageSource Body = LoadImage("Body.png");
	public static readonly ImageSource Head = LoadImage("Head.png");
	public static readonly ImageSource Tail = LoadImage("Tail.png");
	public static readonly ImageSource DeadBody = LoadImage("DeadBody.png");
	public static readonly ImageSource DeadHead = LoadImage("DeadHead.png");
	public static readonly ImageSource Close = LoadImage("close.png");
	public static readonly ImageSource CloseHover = LoadImage("close_h.png");
	public static readonly ImageSource Min = LoadImage("min.png");
	public static readonly ImageSource MinHover = LoadImage("min_h.png");
	
	[SuppressMessage("Performance", "CA1859:Używaj konkretnych typów, aby zwiększyć wydajność – gdy jest to możliwe")]
	private static ImageSource LoadImage(string fileName)
	{
		return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
	}
}
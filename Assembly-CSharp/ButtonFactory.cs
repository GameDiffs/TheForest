using System;

public class ButtonFactory
{
	public static AbstractButton GetPlatformSpecificButtonImplementation()
	{
		return new ClickButton();
	}
}

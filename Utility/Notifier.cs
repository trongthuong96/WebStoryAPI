using org.mozilla.intl.chardet;
using System;

namespace Utility
{
	public class Notifier : nsICharsetDetectionObserver
	{
		public void Notify(string charset)
		{
			CharsetDetector.DetectedCharset = charset;
		}
	}
}

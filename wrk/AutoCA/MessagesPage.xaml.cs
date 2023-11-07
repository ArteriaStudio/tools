using Arteria_s.DB;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Preview.Notes;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	public enum AppFacility
	{
		Unknown = 0,
		Error,
	}

	/// <summary>
	/// 利用者向けメッセージ
	/// </summary>
	public class Message : Data
	{
		public AppFacility	Facility;	//　分類
		public DateTime		CreateAt;	//　日時
		public string		DateAt;		//　
		public string		TimeAt;		//　
		public string		Text;		//　メッセージ

		public Message(AppFacility eFacility, string pText) 
		{
			Facility = eFacility;
			CreateAt = DateTime.Now;
			Text     = pText;

			DateAt = CreateAt.ToString("yyyy/MM/dd");
			TimeAt = CreateAt.ToString("HH:mm:ss.fff");
		}
		

		public override bool Validate()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MessagesPage : Page
	{
		public ObservableCollection<Message>	m_pMessages = new ObservableCollection<Message>();

		public MessagesPage()
		{
			this.InitializeComponent();

#if ENABLE_TRACE
			for (var i = 0; i < 12; i++)
			{
				var pMessage = new Message("ERROR", "エラーです。サンプル文字列");
				pMessage.Text += string.Format("{0}", i);
				m_pMessages.Insert(0, pMessage);
			}
#endif
		}

		//　利用者向けメッセージを追加
		public void	AddMessage(AppFacility eFacility, string pText)
		{
			var pMessage = new Message(eFacility, pText);
			m_pMessages.Insert(0, pMessage);

			return;
		}
		public void AddMessage(Message pMessage)
		{
			m_pMessages.Insert(0, pMessage);

			return;
		}
	}
}

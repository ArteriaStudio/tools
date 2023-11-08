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
using System.Reflection.Metadata;
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
		Info,
		Complete,
	}

	/// <summary>
	/// ���p�Ҍ������b�Z�[�W
	/// </summary>
	public class Message : Data
	{
		public AppFacility	Facility;	//�@����
		public DateTime		CreateAt;	//�@����
		public string		DateAt;		//�@
		public string		TimeAt;		//�@
		public string		Text;		//�@���b�Z�[�W
		public string		Which;		//�@�Ώۂ̎��ʎq
		public string		Where;      //�@�Ώۂ̏ꏊ

		public Message(AppFacility eFacility, string pText, string pWhich, string pWhere = null)
		{
			Facility  = eFacility;
			CreateAt  = DateTime.Now;
			Text      = pText;
			Which     = pWhich;
			Where     = pWhere;

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
				var pMessage = new Message("ERROR", "�G���[�ł��B�T���v��������");
				pMessage.Text += string.Format("{0}", i);
				m_pMessages.Insert(0, pMessage);
			}
#endif
		}

		//�@���p�Ҍ������b�Z�[�W��ǉ�
		public void	AddMessage(AppFacility eFacility, string pText, string pWhich, string pWhere)
		{
			var pMessage = new Message(eFacility, pText, pWhich, pWhere);
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

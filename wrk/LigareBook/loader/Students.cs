using Arteria_s.DB.Base;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LigareBook
{
	public class StudentCSV
	{
		[Index(0)]
		public int Year { get; set; }
		[Index(1)]
		public String School { get; set; }
		[Index(2)]
		public String Grade { get; set; }
		[Index(3)]
		public String Name { get; set; }
		[Index(4)]
		public String Read { get; set; }
		[Index(5)]
		public String Sets { get; set; }
		[Index(6)]
		public String Numbers { get; set; }
		[Index(7)]
		public String Gender { get; set; }
		[Index(8)]
		public DateTime BirthAt { get; set; }
		[Index(9)]
		public String StudentNumber { get; set; }
		[Index(10)]
		public String Email { get; set; }

		public bool CheckSelf()
		{
			if (Year < 2000)
			{
				return(false);
			}
			if (Lexical.IsSchool(School) == false)
			{
				return(false);
			}
			if (Lexical.IsGrade(Grade) == false)
			{
				return (false);
			}
			if (Lexical.IsName(Name) == false)
			{
				return (false);
			}
			if (Lexical.IsRead(Read) == false)
			{
				return (false);
			}
			/*
			if (Lexical.IsNumber(Sets) == false)
			{
				return (false);
			}
			*/
			if (Lexical.IsNumber(Numbers) == false)
			{
				return (false);
			}
			if (Lexical.IsGender(Gender) == false)
			{
				return (false);
			}
			if (Lexical.IsDateTime(BirthAt) == false)
			{
				return (false);
			}
			if (Lexical.IsNumber(StudentNumber) == false)
			{
				return (false);
			}
			if (Lexical.IsEmail(Email) == false)
			{
				return (false);
			}

			return (true);
		}

		public void Dump()
		{
			System.Diagnostics.Debug.WriteLine($"年度: {Year}");
			System.Diagnostics.Debug.WriteLine($"学校区分: {School}");
			System.Diagnostics.Debug.WriteLine($"学年: {Grade}");
			System.Diagnostics.Debug.WriteLine($"名前: {Name}");
			System.Diagnostics.Debug.WriteLine($"ふりがな: {Read}");
			System.Diagnostics.Debug.WriteLine($"クラス: {Sets}");
			System.Diagnostics.Debug.WriteLine($"出席番号: {Numbers}");
			System.Diagnostics.Debug.WriteLine($"性別: {Gender}");
			System.Diagnostics.Debug.WriteLine($"生年月日: ${BirthAt}");
			System.Diagnostics.Debug.WriteLine($"学籍番号: ${StudentNumber}");
			System.Diagnostics.Debug.WriteLine($"e-Mail: ${Email}");
		}
	}

	public interface StudentsLoaderEventListener
	{
		public abstract void OnCheckedStudents(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<StudentCSV> pItems);
	}

	public class StudentsLoader : Loader
	{
		StudentsLoaderEventListener 	m_pListener = null;
		public StudentsLoader(StudentsLoaderEventListener pListener)
		{
			m_pListener = pListener;
		}
		~StudentsLoader()
		{
			m_pListener = null;
		}
		public override bool Load(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var pItems = new List<Student>();
				while (pFetcher.Read())
				{
					var pRecoords = pFetcher.GetRecord<StudentCSV>();
					if (pRecoords.CheckSelf() == true)
					{
						Student 	pStudent = new Student();

//						pStudent.AccountID = pRecoords.AccountID;
						pStudent.Email = pRecoords.Email;
						pStudent.Name = pRecoords.Name;
						pStudent.Read = pRecoords.Read;
//						pStudent.OrgUnitID = pRecoords.OrgUnitID;
//						pStudent.Status = pRecoords.Status;
//						pStudent.ExpireAt = pRecoords.ExpireAt;
//						pStudent.UpdateAt = pRecoords.UpdateAt;
//						pStudent.DeleteAt = pRecoords.DeleteAt;
						pStudent.StudentNumber = pRecoords.StudentNumber;
//						pStudent.EnterAt = pRecoords.EnterAt;
//						pStudent.LeaveAt = pRecoords.LeaveAt;
						pStudent.Year = pRecoords.Year;
						pStudent.School = pRecoords.School;
						pStudent.Grade = pRecoords.Grade;
						pStudent.Sets = pRecoords.Sets;
						pStudent.Numbers = pRecoords.Numbers;
						pStudent.BirthAt = pRecoords.BirthAt;
						pStudent.Gender= pRecoords.Gender;

						pItems.Add(pStudent);
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
					}
				}

				//　データベースに登録
				StudentsCursor.Insert(pContext, pItems);
			}

			return(true);
		}

		public override bool Check(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var nItems = 0;
				var nError = 0;
				var pItems = new List<StudentCSV>();
				while (pFetcher.Read())
				{
					var pRecoords = pFetcher.GetRecord<StudentCSV>();
					if (pRecoords.CheckSelf() == true)
					{
						pItems.Add(pRecoords);
						nItems ++;
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
						nError ++;
					}
				}
				m_pListener.OnCheckedStudents(pPath, pCodeSet, pContext, nItems, nError, pItems);
			}

			return (true);
		}
	}
}

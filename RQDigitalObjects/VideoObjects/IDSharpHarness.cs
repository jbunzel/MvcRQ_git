/*
 *   IdSharp - A tagging library for .NET
 *   Copyright (C) 2007  Jud White
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License along
 *   with this program; if not, write to the Free Software Foundation, Inc.,
 *   51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System;
using System.Collections.Specialized;
using System.Reflection;
//using System.ComponentModel;
//using System.Drawing;
using System.IO;
//using System.Windows.Forms;
using IdSharp.Tagging.ID3v1;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v2.Frames;

namespace RQDigitalObjects.AudioObjects
{
	public class ID3v2Info
	{


		#region <<< Private Fields >>>

		private IID3v2 m_ID3v2;

		private StringDictionary m_tagdict;
		private String m_filename;
		private bool m_containsID3v1;
		private bool m_containsID3v2;
		#endregion


		#region <<< Property Definitions >>>

		/// <summary>
		/// Dateiname des einzulesenden MP3-Files.
		/// </summary>
		public String Filename
		{
			get
			{
				return m_filename;
			}
		}


		/// <summary>
		/// Gibt an, ob die Datei ID3v1-Informationen beinhaltet. Dieses Attribut
		/// ist ReadOnly.
		/// </summary>
		public bool ContainsID3v1
		{
			get
			{
				return this.m_containsID3v1;
			}
		}


		/// <summary>
		/// Gibt an, ob die Datei ID3v2-Informationen beinhaltet. Dieses Attribut
		/// ist ReadOnly.
		/// </summary>
		public bool ContainsID3v2
		{
			get
			{
				return this.m_containsID3v2;
			}
		}


		/// <summary>
		/// Name des Werkes aus den ID3v1-Informationen.
		/// </summary>
		public StringDictionary Tags
		{
			get
			{
				return this.m_tagdict;
			}
			set
			{
				this.m_tagdict = value;
			}
		}


		/// <summary>
		/// Name des Werkes aus den ID3v1-Informationen.
		/// </summary>
		public String Title
		{
			get
			{
				return m_ID3v2.Title;
			}
		}


		/// <summary>
		/// Name des Künstler aus den ID3v1-Informationen.
		/// </summary>
		public String Artist
		{
			get
			{
				return m_ID3v2.Artist;
			}
		}

		
		/// <summary>
		/// Name des Albums aus den ID3v1-Informationen.
		/// </summary>
		public String Album
		{
			get
			{
				return m_ID3v2.Album;
			}
		}


		/// <summary>
		/// Erscheinungsjahr aus den ID3v1-Informationen.
		/// </summary>
		public String Year
		{
			get
			{
				return m_ID3v2.Year;
			}
		}


		/// <summary>
		/// Genre aus den ID3v1-Informationen.
		/// </summary>
		public string Genre
		{
			//Intern wird im File nur ein Byte-Wert als Genre-Info
			//gespeichert. Die Übersetzung in Klarnamen erfolgt mit einer
			//internen Hash-Tabelle.

			get
			{
				return m_ID3v2.Genre;
			}
		}

		/// <summary>
		/// CD-Track-Nummer aus den ID3v1-Informationen.
		/// </summary>
		public String Track
		{
			get
			{
				return m_ID3v2.TrackNumber;
			}
		}

		/// <summary>
		/// Name des Komponisten aus den ID3v2-Informationen.
		/// </summary>
		public String Composer
		{
			get
			{
				return m_ID3v2.Composer;
			}
		}


		/// <summary>
		/// Name des Dirigenten aus den ID3v2-Informationen.
		/// </summary>
		public String Conductor
		{
			get
			{
				return m_ID3v2.Conductor;
			}
		}


		/// <summary>
		/// Name des Aufnahmedatums aus den ID3v2-Informationen.
		/// </summary>
		public String DateRecorder
		{
			get
			{
				return m_ID3v2.DateRecorded;
			}
		}


		/// <summary>
		/// Name der Platten-Nummer aus den ID3v2-Informationen.
		/// </summary>
		public String DiscNumber
		{
			get
			{
				return m_ID3v2.DiscNumber;
			}
		}


		#endregion


		#region <<< Constructor >>>

		public ID3v2Info()
		{
			//InitializeComponent();

			//cmbGenre.Sorted = true;
			//cmbGenre.Items.AddRange(GenreHelper.GenreByIndex);

			//cmbImageType.Items.AddRange(PictureTypeHelper.PictureTypes);
		}

		#endregion <<< Constructor >>>


		#region <<< Private Methods >>>
		/*
        private void LoadImageData(IAttachedPicture attachedPicture)
        {
            pictureBox1.Image = attachedPicture.Picture;

            txtImageDescription.Text = attachedPicture.Description;
            cmbImageType.SelectedIndex = cmbImageType.Items.IndexOf(PictureTypeHelper.GetStringFromPictureType(attachedPicture.PictureType));

            txtImageDescription.Enabled = true;
            cmbImageType.Enabled = true;
        }

        private void ClearImageData()
        {
            pictureBox1.Image = null;
            txtImageDescription.Text = "";
            cmbImageType.SelectedIndex = -1;

            txtImageDescription.Enabled = false;
            cmbImageType.Enabled = false;
        }

        private void SaveImageToFile(IAttachedPicture attachedPicture)
        {
            String extension = attachedPicture.PictureExtension;

            imageSaveFileDialog.FileName = "image." + extension;

            DialogResult dialogResult = imageSaveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                using (FileStream fs = File.Open(imageSaveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fs.Write(attachedPicture.PictureData, 0, attachedPicture.PictureData.Length);
                }
            }
        }

        private void LoadImageFromFile(IAttachedPicture attachedPicture)
        {
            DialogResult dialogResult = imageOpenFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                attachedPicture.Picture = Image.FromFile(imageOpenFileDialog.FileName);
                pictureBox1.Image = attachedPicture.Picture;
            }
        }

        private IAttachedPicture GetCurrentPictureFrame()
        {
            if (imageBindingNavigator.BindingSource == null)
                return null;
            return imageBindingNavigator.BindingSource.Current as IAttachedPicture;
        }
*/
		#endregion <<< Private Methods >>>


		#region <<< Public Methods >>>

		public void LoadFile(FileInfo file)
		{
			m_ID3v2 = ID3v2Helper.CreateID3v2(file.FullName);
			m_tagdict = new StringDictionary();
			m_tagdict.Add("MP3FileName", file.Name);
			
			PropertyInfo[] infos = this.m_ID3v2.GetType().GetProperties();

			foreach (PropertyInfo info in infos)
			{
				m_tagdict.Add(info.Name, String.Format("{0}", info.GetValue(this.m_ID3v2,null )));
			}
			switch (m_ID3v2.Header.TagVersion)
			{
					case ID3v2TagVersion.ID3v22:
							break;
					case ID3v2TagVersion.ID3v23:
							break;
					case ID3v2TagVersion.ID3v24:
							break;
			}
		}


		public void SaveFile(FileInfo file)
		{

			PropertyInfo[] infos = this.m_ID3v2.GetType().GetProperties();

			foreach (PropertyInfo info in infos)
			{
				if (info.PropertyType.Name=="String")
					info.SetValue(this.m_ID3v2, (object)m_tagdict[info.Name], null);
			}
			m_ID3v2.Save(file.FullName);
			/*
									if (m_ID3v2 == null)
									{
											MessageBox.Show("Nothing to save!");
											return;
									}

									if (cmbID3v2.SelectedIndex == cmbID3v2.Items.IndexOf("ID3v2.2"))
											m_ID3v2.Header.TagVersion = ID3v2TagVersion.ID3v22;
									else if (cmbID3v2.SelectedIndex == cmbID3v2.Items.IndexOf("ID3v2.3"))
											m_ID3v2.Header.TagVersion = ID3v2TagVersion.ID3v23;
									else if (cmbID3v2.SelectedIndex == cmbID3v2.Items.IndexOf("ID3v2.4"))
											m_ID3v2.Header.TagVersion = ID3v2TagVersion.ID3v24;
									else
											throw new Exception("Unknown tag version");

									m_ID3v2.Artist = txtArtist.Text;
									m_ID3v2.Title = txtTitle.Text;
									m_ID3v2.Album = txtAlbum.Text;
									m_ID3v2.Genre = cmbGenre.Text;
									m_ID3v2.Year = txtYear.Text;
									m_ID3v2.TrackNumber = txtTrackNumber.Text;

									m_ID3v2.Save(path);
			 */
		}

		public string GetTagValue(string key)
		{
			if (m_tagdict.ContainsKey(key))
				return m_tagdict[key];
			else
				return "N/A";
		}

		#endregion <<< Public Methods >>>

	}
}

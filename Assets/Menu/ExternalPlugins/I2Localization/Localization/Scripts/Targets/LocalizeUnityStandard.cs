﻿using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc
{
	public partial class Localize
	{
		#region Cache

		Text 	    mTarget_Text;
		TextMesh 	mTarget_TextMesh;
		AudioSource mTarget_AudioSource;
		Image 	    mTarget_Image;
		GameObject  mTarget_Child;
		bool mInitializeAlignment = true;
		TextAlignment mOriginalAlignmentStd = TextAlignment.Left;
        TextAnchor mOriginalAlignmentAnchor = TextAnchor.UpperLeft;

        public void RegisterEvents_UnityStandard()
		{
			EventFindTarget += FindTarget_GUIText;
			EventFindTarget += FindTarget_TextMesh;
			EventFindTarget += FindTarget_AudioSource;
			EventFindTarget += FindTarget_GUITexture;
			EventFindTarget += FindTarget_Child;
		}

		#endregion

		#region Find Target

		void FindTarget_GUIText() 		{ FindAndCacheTarget (ref mTarget_Text, 		SetFinalTerms_GUIText,		DoLocalize_GUIText,		true, true, false); }
		void FindTarget_TextMesh() 		{ FindAndCacheTarget (ref mTarget_TextMesh,		SetFinalTerms_TextMesh,		DoLocalize_TextMesh,	true, true, false); }
		void FindTarget_AudioSource()	{ FindAndCacheTarget (ref mTarget_AudioSource,	SetFinalTerms_AudioSource,	DoLocalize_AudioSource,	false,false, false);}
		void FindTarget_GUITexture() 	{ FindAndCacheTarget (ref mTarget_Image,	SetFinalTerms_GUITexture,	DoLocalize_GUITexture,	false,false, false);}
		void FindTarget_Child() 		{ FindAndCacheTarget (ref mTarget_Child,		SetFinalTerms_Child,		DoLocalize_Child,		false,false, false);}

		#endregion

		#region SetFinalTerms

		public void SetFinalTerms_GUIText(string Main, string Secondary, out string PrimaryTerm, out string SecondaryTerm)
		{
			if (string.IsNullOrEmpty(Secondary) && mTarget_Text.font!=null)
				Secondary = mTarget_Text.font.name;
			SetFinalTerms (mTarget_Text.text, 	Secondary, 	out PrimaryTerm, out SecondaryTerm, true);
		}

		public void SetFinalTerms_TextMesh(string Main, string Secondary, out string PrimaryTerm, out string SecondaryTerm)
		{
			string second = (mTarget_TextMesh.font!=null ? mTarget_TextMesh.font.name : string.Empty);
			SetFinalTerms (mTarget_TextMesh.text, 	second, out PrimaryTerm, out SecondaryTerm, true);
		}

		public void SetFinalTerms_GUITexture(string Main, string Secondary, out string PrimaryTerm, out string SecondaryTerm)
		{
			if (!mTarget_Image || !mTarget_Image.sprite.texture) 
			{
				SetFinalTerms( string.Empty, string.Empty, out PrimaryTerm, out SecondaryTerm, false );
			}
			else
			{
				SetFinalTerms (mTarget_Image.sprite.texture.name,	string.Empty, 		out PrimaryTerm, out SecondaryTerm, false);
			}
		}

		public void SetFinalTerms_AudioSource(string Main, string Secondary, out string PrimaryTerm, out string SecondaryTerm)
		{
			if (!mTarget_AudioSource || !mTarget_AudioSource.clip)
			{
				SetFinalTerms( string.Empty, string.Empty, out PrimaryTerm, out SecondaryTerm, false );
			}
			else
			{
				SetFinalTerms (mTarget_AudioSource.clip.name,string.Empty, 		out PrimaryTerm, out SecondaryTerm, false);
			}
		}

		public void SetFinalTerms_Child(string Main, string Secondary, out string PrimaryTerm, out string SecondaryTerm)
		{
			SetFinalTerms (mTarget_Child.name,	string.Empty, 	out PrimaryTerm, out SecondaryTerm, false);
		}

		#endregion

		#region DoLocalize

		void DoLocalize_GUIText( string MainTranslation, string SecondaryTranslation )
		{
			//--[ Localize Font Object ]----------
			Font newFont = GetSecondaryTranslatedObj<Font>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null && mTarget_Text.font != newFont) 
				mTarget_Text.font = newFont;

			//--[ Localize Text ]----------
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
                mOriginalAlignmentAnchor = mTarget_Text.alignment;
			}
			if (!string.IsNullOrEmpty(MainTranslation) && mTarget_Text.text != MainTranslation)
			{
				if (CurrentLocalizeComponent.CorrectAlignmentForRTL)
					mTarget_Text.alignment = mOriginalAlignmentAnchor  == TextAnchor.UpperLeft && LocalizationManager.IsRight2Left ? TextAnchor.UpperRight : mOriginalAlignmentAnchor;
				
				mTarget_Text.text = MainTranslation;
			}
		}
		
		void DoLocalize_TextMesh( string MainTranslation, string SecondaryTranslation )
		{
			//--[ Localize Font Object ]----------
			Font newFont = GetSecondaryTranslatedObj<Font>(ref MainTranslation, ref SecondaryTranslation);
			if (newFont!=null && mTarget_TextMesh.font != newFont)
			{
				mTarget_TextMesh.font = newFont;
				GetComponent<Renderer>().sharedMaterial = newFont.material;
			}
			
			//--[ Localize Text ]----------
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
                mOriginalAlignmentStd = mTarget_TextMesh.alignment;
			}
			if (!string.IsNullOrEmpty(MainTranslation) && mTarget_TextMesh.text != MainTranslation)
			{
				if (CurrentLocalizeComponent.CorrectAlignmentForRTL)
					mTarget_TextMesh.alignment = mOriginalAlignmentStd == TextAlignment.Left && LocalizationManager.IsRight2Left ? TextAlignment.Right : mOriginalAlignmentStd;

				mTarget_TextMesh.text = MainTranslation;
			}
		}

		void DoLocalize_AudioSource( string MainTranslation, string SecondaryTranslation )
		{
			bool bIsPlaying = mTarget_AudioSource.isPlaying;
			AudioClip OldClip = mTarget_AudioSource.clip;
			AudioClip NewClip = FindTranslatedObject<AudioClip> (MainTranslation);
			if (OldClip != NewClip)
				mTarget_AudioSource.clip = NewClip;

			if (bIsPlaying && mTarget_AudioSource.clip) 
				mTarget_AudioSource.Play();
			
			// If the old clip is not in the translatedObjects, then unload it as it most likely was loaded from Resources
			//if (!HasTranslatedObject(OldClip))
			//	Resources.UnloadAsset(OldClip);
		}
		
		void DoLocalize_GUITexture( string MainTranslation, string SecondaryTranslation )
		{
			Texture Old = mTarget_Image.sprite.texture;
			if (Old!=null && Old.name!=MainTranslation)
				mTarget_Image.sprite = FindTranslatedObject<Texture2D>(MainTranslation).ToSprite();
			
			// If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
			//if (!HasTranslatedObject(Old))
			//	Resources.UnloadAsset(Old);
		}
		
		void DoLocalize_Child( string MainTranslation, string SecondaryTranslation )
		{
			if (mTarget_Child && mTarget_Child.name==MainTranslation)
				return;

			GameObject current = mTarget_Child;
			GameObject NewPrefab = FindTranslatedObject<GameObject>(MainTranslation);
			if (NewPrefab)
			{
				mTarget_Child = (GameObject)Instantiate( NewPrefab );
				Transform mNew = mTarget_Child.transform;
				Transform bBase = (current ? current.transform : NewPrefab.transform );
				
				mNew.parent = transform;
				mNew.localScale    = bBase.localScale;
				mNew.localRotation = bBase.localRotation;
				mNew.localPosition = bBase.localPosition;
			}
			if (current)
			{
				#if UNITY_EDITOR
					DestroyImmediate (current);
				#else
					Destroy (current);
				#endif
			}
		}

		#endregion
	}
}
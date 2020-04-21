using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Player_Emitter : MonoBehaviour
{
    private EventInstance event_P_Mov_Footsteps;
    private PARAMETER_ID groundMaterialParameterId;

    private EventInstance event_Book_Close;
    private EventInstance event_Book_Open;
    private EventInstance event_Book_Page;

    [SerializeField]
    private Player_Data player_Data;

    private void Awake()
    {
        event_P_Mov_Footsteps = RuntimeManager.CreateInstance(player_Data.p_mov_rnd_footsteps);
            EventDescription groundMaterialEventDescription;
            event_P_Mov_Footsteps.getDescription(out groundMaterialEventDescription);
            PARAMETER_DESCRIPTION groundMaterialParameterDescription;
            groundMaterialEventDescription.getParameterDescriptionByName("ground_material", out groundMaterialParameterDescription);
            groundMaterialParameterId = groundMaterialParameterDescription.id;
    }

    public void Init_Footsteps(float ground_material)
    {
        event_P_Mov_Footsteps.setParameterByID(groundMaterialParameterId, ground_material);
        event_P_Mov_Footsteps.start();
    }

    public void Init_Book_Open()
    {
        event_Book_Close = RuntimeManager.CreateInstance(player_Data.p_book_close);
        event_Book_Open = RuntimeManager.CreateInstance(player_Data.p_book_open);
        event_Book_Page = RuntimeManager.CreateInstance(player_Data.p_book_page);
        event_Book_Open.start();
    }

    public void Init_Book_Page()
    {
        event_Book_Page.start();
    }

    public void Init_Book_Close()
    {
        event_Book_Close.start();

        event_Book_Open.release();
        event_Book_Page.release();
        event_Book_Close.release();
    }

    //private void Start()
    //{
    //    StartCoroutine(TestSound());
    //}

    //private IEnumerator TestSound()
    //{
    //    yield return new WaitForSeconds(1);
    //    Init_Book_Open();
    //    yield return new WaitForSeconds(2);
    //    Init_Book_Page();
    //    yield return new WaitForSeconds(1);
    //    Init_Book_Page();
    //    yield return new WaitForSeconds(2);
    //    Init_Book_Close();
    //}
}

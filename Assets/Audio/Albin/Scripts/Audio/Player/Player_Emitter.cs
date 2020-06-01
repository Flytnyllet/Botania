using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Player_Emitter : MonoBehaviour
{
    private EventInstance event_P_Mov_Footsteps;
    private PARAMETER_ID groundMaterialParameterId;

    private EventInstance event_P_Mov_Swim;
    private PARAMETER_ID underwaterParameterId;

    private EventInstance event_P_Mov_Jump;

    private EventInstance event_P_Mov_Land;

    private EventInstance event_Book_Close;
    private EventInstance event_Book_Open;
    private EventInstance event_Book_Page;

    [SerializeField]
    private Player_Data player_Data;
    private FPSMovement _movement;

    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.OPEN_BOOK, Init_Book_Open);
        EventManager.Subscribe(EventNameLibrary.CLOSE_BOOK, Init_Book_Close);
        EventManager.Subscribe(EventNameLibrary.FLIP_PAGE, Init_Book_Page);
        //EventManager.Subscribe(EventNameLibrary.DRINK_POTION,);
        EventManager.Subscribe(EventNameLibrary.CREATE_POTION, Init_Book_Page);
        EventManager.Subscribe(EventNameLibrary.TELEPOT, Init_Book_Page);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.OPEN_BOOK, Init_Book_Open);
        EventManager.UnSubscribe(EventNameLibrary.CLOSE_BOOK, Init_Book_Close);
        EventManager.UnSubscribe(EventNameLibrary.FLIP_PAGE, Init_Book_Page);
    }

    private void Start()
    {   
        _movement = GetComponentInParent<FPSMovement>();
        event_P_Mov_Footsteps = RuntimeManager.CreateInstance(player_Data.p_mov_rnd_footsteps);
        event_P_Mov_Swim = RuntimeManager.CreateInstance(player_Data.p_mov_swim);
        event_P_Mov_Jump = RuntimeManager.CreateInstance(player_Data.p_mov_jump);
        event_P_Mov_Land = RuntimeManager.CreateInstance(player_Data.p_mov_land);

        event_Book_Close = RuntimeManager.CreateInstance(player_Data.p_book_close);
        event_Book_Open = RuntimeManager.CreateInstance(player_Data.p_book_open);
        event_Book_Page = RuntimeManager.CreateInstance(player_Data.p_book_page);

        EventDescription groundMaterialEventDescription;
        event_P_Mov_Footsteps.getDescription(out groundMaterialEventDescription);
        PARAMETER_DESCRIPTION groundMaterialParameterDescription;
        groundMaterialEventDescription.getParameterDescriptionByName("ground_material", out groundMaterialParameterDescription);
        groundMaterialParameterId = groundMaterialParameterDescription.id;

        EventDescription underwaterEventDescription;
        event_P_Mov_Swim.getDescription(out underwaterEventDescription);
        PARAMETER_DESCRIPTION underwaterParameterDescription;
        underwaterEventDescription.getParameterDescriptionByName("underwater", out underwaterParameterDescription);
        underwaterParameterId = underwaterParameterDescription.id;
    }

    public void Init_Footsteps(float ground_material)
    {
        event_P_Mov_Footsteps.setParameterByID(groundMaterialParameterId, ground_material);
        event_P_Mov_Footsteps.start();
    }

    public void Init_Swim(float underwater)
    {
        event_P_Mov_Swim.setParameterByID(underwaterParameterId, underwater);
        event_P_Mov_Swim.start();
    }

    public void Init_Jump()
    {
        event_P_Mov_Jump.start();
    }

    public void Init_Land()
    {
        event_P_Mov_Land.start();
    }

    public void Init_Book_Open(EventParameter param)
    {
        event_Book_Open.start();
    }

    public void Init_Book_Page(EventParameter param)
    {
        event_Book_Page.start();
    }

    public void Init_Book_Close(EventParameter param)
    {
        event_Book_Close.start();
    }

    public void Init_Pickup(string event_Ref)
    {
        RuntimeManager.PlayOneShot(event_Ref, transform.position);
    }
}

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
    private EventInstance event_Book_Scribble;
    private EventInstance event_Potion_Create;
    private EventInstance event_Potion_Drink;
    private EventInstance event_Potion_Teleportation;

    [SerializeField]
    private Player_Data player_Data;
    private FPSMovement _movement;

    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.OPEN_BOOK, Init_Book_Open);
        EventManager.Subscribe(EventNameLibrary.CLOSE_BOOK, Init_Book_Close);
        EventManager.Subscribe(EventNameLibrary.FLIP_PAGE, Init_Book_Page);
        EventManager.Subscribe(EventNameLibrary.DRINK_POTION,Init_Potion_Drink);
        EventManager.Subscribe(EventNameLibrary.CREATE_POTION, Init_Potion_Create);
        EventManager.Subscribe(EventNameLibrary.TELEPOT, Init_Potion_Teleportation);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.OPEN_BOOK, Init_Book_Open);
        EventManager.UnSubscribe(EventNameLibrary.CLOSE_BOOK, Init_Book_Close);
        EventManager.UnSubscribe(EventNameLibrary.FLIP_PAGE, Init_Book_Page);
        EventManager.UnSubscribe(EventNameLibrary.DRINK_POTION, Init_Potion_Drink);
        EventManager.UnSubscribe(EventNameLibrary.CREATE_POTION, Init_Potion_Create);
        EventManager.UnSubscribe(EventNameLibrary.TELEPOT, Init_Potion_Teleportation);
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
        event_Book_Scribble = RuntimeManager.CreateInstance(player_Data.p_book_scribble);
        event_Potion_Create = RuntimeManager.CreateInstance(player_Data.p_potion_create);
        event_Potion_Teleportation = RuntimeManager.CreateInstance(player_Data.p_potion_teleport);
        event_Potion_Drink = RuntimeManager.CreateInstance(player_Data.p_potion_drink);

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

    //========== MOVEMENT =============

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

    //========== INVENTORY =============

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

    public void Init_Book_Scribble()
    {
        event_Book_Scribble.start();
    }

    public void Init_Pickup(string event_Ref)
    {
        RuntimeManager.PlayOneShot(event_Ref, transform.position);
    }

    //======== POTIONS ========

    public void Init_Potion_Create(EventParameter param = null)
    {
        event_Potion_Create.start();
    }

    public void Init_Potion_Drink(EventParameter param = null)
    {
        event_Potion_Drink.start();
    }

    public void Init_Potion_Teleportation(EventParameter param = null)
    {
        event_Potion_Teleportation.start();
    }
}

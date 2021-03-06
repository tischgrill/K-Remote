﻿using K_Remote.Resources;
using K_Remote.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace K_Remote.Models
{
    /**
     * Response of getActivePlayers
     */
    class ActivePlayers
    {
        public int id;
        public string jsonrpc;
        public Player[] result;

        public ActivePlayers(int id, string jsonrpc, Player[] result)
        {
            this.id = id;
            this.jsonrpc = jsonrpc;
            this.result = result;
        }
    }

    /**
     * Model of a player
     * Playerid:  0=music, 1=video, 2=pictures
     */
    class Player
    {
        public int playerId;
        public string type;
        public int speed;
    }

    class PlayerPropertiesResponse
    {
        public string id;
        public string jsonrpc;
        public PlayerProperties result;
    }

    class PlayerProperties
    {
        public int speed;
    }

    class PlayerItemResponse
    {
        public string id;
        public string jsonrpc;
        public PlayerItemResult result;
    }

    class PlayerItemResult
    {
        public PlayerItem item;

    }

    class PlayerItem : INotifyPropertyChanged
    {
        //properties

        /// <summary>
        /// Album name
        /// </summary>
        public string album { get; set; }

        /// <summary>
        /// A private field holding artist property value
        /// </summary>
        private string[] m_artist;

        /// <summary>
        /// Artists
        /// </summary>
        public string[] artist
        {
            get
            {
                return m_artist;
            }
            set
            {
                m_artist = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(artistProperty)));
            }
        }

        private string m_foreground;

        /// <summary>
        /// Read-only property holding a color string
        /// </summary>
        public string foreground
        {
            get
            {
                return m_foreground;
            }
            private set
            {
                m_foreground = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(foreground)));
            }
        }


        /// <summary>
        /// Private field holding currentlyPlayed value
        /// </summary>
        private bool m_currentlyPlayed = false;

        /// <summary>
        /// Determines if the item is currently played
        /// </summary>
        public bool currentlyPlayed
        {
            get
            {
                return m_currentlyPlayed;
            }
            set
            {
                if (value == true)
                {
                    foreground = Application.Current.Resources["SystemAccentColor"].ToString();
                    playIndicator = Constants.ICON_NOW_PLAYED;
                }
                else
                {
                    foreground = Application.Current.Resources["SystemBaseHighColor"].ToString(); 
                    playIndicator = "";
                }
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(foreground)));
            }
        }

        /// <summary>
        /// Read-only property containing all artists in a comma separated string
        /// </summary>
        public string artistProperty
        {   get
            {
                if(artist == null)
                {
                    return null;
                }
                else
                {
                    return string.Join(", ", artist);
                }
            }
        }

        /// <summary>
        /// A read-only property containing showtitle, season and episode
        /// </summary>
        public string seriesProperty
        {
            get
            {
                if(type == "episode")
                {
                    return showtitle + " S" +  season + "E" + episode;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Private field, holding value of pickedInListViewString and pickedInListViewBool
        /// </summary>
        private string m_addButtonsVisibility = "Collapsed";

        /// <summary>
        /// Determines if the item is picked in playlist ListViews
        /// </summary>
        public string addButtonsVisibility
        {
            get
            {
                return m_addButtonsVisibility;
            }
            set
            {
                if(value == "Visible"|| value == "Collapsed")
                {                    
                    m_addButtonsVisibility = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(addButtonsVisibility)));                    
                }
            }
        }

        private string m_playIndicator = "";

        public string playIndicator
        {
            get
            {
                return m_playIndicator;
            }
            private set
            {
                m_playIndicator = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(playIndicator)));
            }
        }

        /// <summary>
        /// Determines if the item is picked in playlist ListViews
        /// </summary>
        public bool pickedInListView
        {
            get
            {
                if (addButtonsVisibility == "Visible")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)
                {
                    addButtonsVisibility = "Visible";
                }
                else
                {
                    addButtonsVisibility = "Collapsed";
                }
            }
        }

        /// <summary>
        /// Private field holding item title
        /// </summary>
        private string m_title;
        /// <summary>
        /// Title property
        /// </summary>
        public string title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(title)));
            }
        }

        //fields
        public int episode;
        public string fanart;
        public string file;
        public string id = "";
        public string label;
        public int season;
        public string showtitle;
        public string thumbnail;
        public int tvshowid;
        public int duration;

        public string uniqueid;
        public string type;
        public StreamDetails streamdetails;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Checks if two PlayerItems are equal
        /// </summary>
        /// <param name="obj">The object to be checked</param>
        /// <returns>true if obj is an instance of PlayerItem and represents the same media item</returns>
        public override bool Equals(object obj)
        {
            
            if(obj == null || obj.GetType() != typeof(PlayerItem))
            {
                Debug.WriteLine("PlayerItem.equals: invalid object");
                return false;
            }
            PlayerItem item = obj as PlayerItem;
            
            switch (type)
            {
                case "episode":
                    return title == item.title && episode == item.episode && season == item.season && showtitle == item.showtitle;
                case "movie":
                    return title == item.title;
                case "song":
                    return title == item.title && album == item.album && artistProperty == item.artistProperty;
                default:
                    return Tools.StripTagsAndParantheses(title) == Tools.StripTagsAndParantheses(item.title);
            }
        }

        /// <summary>
        /// Copy properties from second playeritem to this
        /// </summary>
        /// <param name="item">Second item</param>
        public void copyProperties(PlayerItem item)
        {
            if(item != null)
            {
                title = item.title;
                episode = item.episode;
                season = item.season;
                showtitle = item.showtitle;
                artist = item.artist;
                album = album;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>Mediatype and title</returns>
        public override string ToString()
        {
            return type + ": " + title;
        }
    }

    class StreamDetails
    {
        public StreamDetailItem[] audio;
        public StreamDetailItem[] subtitle;
        public StreamDetailItem[] video;
    }

    class StreamDetailItem
    {
        public float aspect;
        public string channels;
        public string codec;
        public int duration;
        public int height;
        public int width;
        public string language;
        public string stereomode;
    }
    

    class PlayerState
    {
        public int playerId;
        public int playerSpeed;

        public PlayerState(int playerId, int playerSpeed)
        {
            this.playerId = playerId;
            this.playerSpeed = playerSpeed;
        }
    }

}

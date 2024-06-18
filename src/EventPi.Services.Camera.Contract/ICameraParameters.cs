namespace EventPi.Services.Camera.Contract;

public interface ICameraParametersReadOnly
{
     int Shutter { get;  }
     float AnalogueGain { get;  }
     float ExposureLevel { get; }
    float DigitalGain { get;  }
     float Contrast { get;  }
     float Sharpness { get;  }
     float Brightness { get;  }
     float BlueGain { get;  }
     float RedGain { get;  }
     HdrModeEnum HdrMode { get;  }
     ColormapTypes ColorMap { get; }
    bool AutoHistogramEnabled { get;  }
}
public interface ICameraParameters
{
     int Shutter { get; set; }

    HdrModeEnum HdrMode{ get; set; }
    ColormapTypes ColorMap{ get; set; }
    float ExposureLevel { get; set; }
    float DigitalGain { get; set; }
     float Contrast { get; set; }
     float Sharpness { get; set; }
     float Brightness { get; set; }
     float BlueGain { get; set; }
     float RedGain { get; set; }
     float AnalogueGain { get; set; }
     bool AutoHistogramEnabled{ get; set; }


}


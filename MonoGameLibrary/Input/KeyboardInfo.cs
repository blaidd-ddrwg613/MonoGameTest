using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class KeyboardInfo
{
    // Use continuous state checks (IsKeyDown/IsKeyUp) for actions that should repeat while a key is held, like movement.
    // Use single-frame checks (WasKeyJustPressed/WasKeyJustReleased) for actions that should happen once per key press,
    // like jumping or shooting.
    
    /// <summary>
    /// Stores the state of the keyboard from the previous update cycle.
    /// </summary>
    public KeyboardState PreviousState { get; private set; }
    
    /// <summary>
    /// Stores the state of the keyboard for the current update cycle.
    /// </summary>
    public KeyboardState CurrentState { get; private set; }

    /// <summary>
    /// Creates a new KeyboardInfo.
    /// </summary>
    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Updates the state information of the keyboard.
    /// </summary>
    public void Upadte()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Returns a value that indicates if the key was pressed this frame.
    /// </summary>
    /// <param name="key"> The key to check. </param>
    /// <returns> Returns true if the key is being pressed, otherwise returns false. </returns>
    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>
    /// Returns a Value that indicates if the key was released this frame.
    /// </summary>
    /// <param name="key"> The key to check. </param>
    /// <returns> True if the key was released, otherwise returns false. </returns>
    public bool IsKeyUp(Keys key)
    {
        return CurrentState.IsKeyUp(key);
    }
    
    /// <summary>
    /// Returns a value that indicates if the specified key was just pressed on the current frame.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>true if the specified key was just pressed on the current frame; otherwise, false.</returns>
    public bool WasKeyJustPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }

    /// <summary>
    /// Returns a value that indicates if the specified key was just released on the current frame.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>true if the specified key was just released on the current frame; otherwise, false.</returns>
    public bool WasKeyJustReleased(Keys key)
    {
        return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
    }
}
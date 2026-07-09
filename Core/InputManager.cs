using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum KeyState
{
    None,
    Down,
    Pressed,
    Up
}

public enum KeyCode
{
    Q, W, E, R, T, Y, U, I, O, P,
    A, S, D, F, G, H, J, K, L,
    Z, X, C, V, B, N, M,

    Up,
    Down,
    Left,
    Right,
    Space,

    MouseLeft,
    MouseRight,

    END
}

public class Key
{
    public KeyCode key;
    public KeyState state;
    public bool pressed;
}

public static class InputManager
{
    /*
     *  C# 등의 닷넷(C#/.NET) 프로그램에서 Windows 운영체제의 핵심 시스템 파일인 
     *  user32.dll 안에 들어있는 C/C++ 기반의 네이티브 함수(Win32 API)를 호출하기 위해 사용하는 속성(Attribute)
     */

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey); //Windows OS 시스템의 기능

    private const int VK_UP = 0x26;
    private const int VK_DOWN = 0x28;
    private const int VK_LEFT = 0x25;
    private const int VK_RIGHT = 0x27;
    private const int VK_SPACE = 0x20;
    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;

    private static readonly int[] ASCII =
    {
        'Q','W','E','R','T','Y','U','I','O','P',
        'A','S','D','F','G','H','J','K','L',
        'Z','X','C','V','B','N','M',

        VK_UP,
        VK_DOWN,
        VK_LEFT,
        VK_RIGHT,
        VK_SPACE,

        VK_LBUTTON,
        VK_RBUTTON
    };

    private static readonly List<Key> m_vecKeys = new();

    public static void Init()
    {
        for (int i = 0; i < (int)KeyCode.END; i++)
        {
            m_vecKeys.Add(new Key()
            {
                key = (KeyCode)i,
                state = KeyState.None,
                pressed = false
            });
        }
    }

    public static void Update()
    {
        for (int i = 0; i < (int)KeyCode.END; i++)
        {
            if ((GetAsyncKeyState(ASCII[i]) & 0x8000) != 0)//https://learn.microsoft.com/ko-kr/windows/win32/api/winuser/nf-winuser-getasynckeystate
            {
                // 이전 프레임에도 눌려있었다.
                if (m_vecKeys[i].pressed)
                    m_vecKeys[i].state = KeyState.Pressed;
                else
                    m_vecKeys[i].state = KeyState.Down;

                m_vecKeys[i].pressed = true;
            }
            else
            {
                // 이전 프레임에 눌려있었다.
                if (m_vecKeys[i].pressed)
                    m_vecKeys[i].state = KeyState.Up;
                else
                    m_vecKeys[i].state = KeyState.None;

                m_vecKeys[i].pressed = false;
            }
        }
    }

    public static bool GetKey(KeyCode key)
    {
        return m_vecKeys[(int)key].state == KeyState.Pressed;
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return m_vecKeys[(int)key].state == KeyState.Down;
    }

    public static bool GetKeyUp(KeyCode key)
    {
        return m_vecKeys[(int)key].state == KeyState.Up;
    }

    public static KeyState GetKeyState(KeyCode key)
    {
        return m_vecKeys[(int)key].state;
    }
}
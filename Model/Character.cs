using ConsoleGameFramework.Core;
using ConsoleGameFramework_KR.Model;
using System;
using System.Numerics;

namespace ConsoleGameFramework.Models;


public class Entity
{
    public string Name { get; private set; }
    public Vec2 m_vPos;

    public Layer m_eLayer; //내 레이어
    public Layer m_eLayerMask; //내가 못가는 지역 마스크


    public Entity(Vec2 pos, Layer layer, Layer eLayerMask)
    {
        m_vPos = pos;
        m_eLayer = layer;
        m_eLayerMask = eLayerMask;
    }

    public virtual void Update(GameContext context)
    {

    }
    public virtual void Render(GameContext context)
    {

    }
}


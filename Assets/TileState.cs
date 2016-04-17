public struct TileState
{
    public Triangle first;
    public Triangle second;

    public bool canEnterFrom(Level.MoveDir dir, Triangle.Alignment align)
    {
        if (first == null)
            return true;
        else if (second == null)
        {
            //check if the Triangles could fit together.
            if ((int)first.direction != ((int)align + 2) % 4)
                return false;
            //check if the triangle is moving in the correct direction to slot-in
            switch (align)
            {
                case Triangle.Alignment.SW:
                    if (dir == Level.MoveDir.up || dir == Level.MoveDir.right)
                        return true;
                    break;
                case Triangle.Alignment.SE:
                    if (dir == Level.MoveDir.up || dir == Level.MoveDir.left)
                        return true;
                    break;
                case Triangle.Alignment.NE:
                    if (dir == Level.MoveDir.down || dir == Level.MoveDir.left)
                        return true;
                    break;
                case Triangle.Alignment.NW:
                    if (dir == Level.MoveDir.down || dir == Level.MoveDir.right)
                        return true;
                    break;
            }
        }
        return false;
    }

    /// <summary>
    /// true if the second triangle in this tile is blocking exit.
    /// </summary>
    /// <param name="dir"> the direction that the triangle wants to move</param>
    /// <param name="align"> the alignment of the triangle that is moving </param>
    /// <returns></returns>
    public bool canExitTo(Level.MoveDir dir, Triangle.Alignment align)
    {
        if (first == null)
            return true;
        switch (align)
        {
            case Triangle.Alignment.SW:
                if (dir == Level.MoveDir.down || dir == Level.MoveDir.left)
                    return true;
                break;
            case Triangle.Alignment.SE:
                if (dir == Level.MoveDir.down || dir == Level.MoveDir.right)
                    return true;
                break;
            case Triangle.Alignment.NE:
                if (dir == Level.MoveDir.up || dir == Level.MoveDir.right)
                    return true;
                break;
            case Triangle.Alignment.NW:
                if (dir == Level.MoveDir.up || dir == Level.MoveDir.left)
                    return true;
                break;
        }
        return false;
    }
}
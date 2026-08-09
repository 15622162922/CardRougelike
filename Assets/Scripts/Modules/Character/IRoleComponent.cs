using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoleComponent
{
    public void Bind(Role role);

    public void Unbind();
}

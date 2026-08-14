import glfw
from OpenGL.GL import *


WIDTH = 800
HEIGHT = 600

STEP = 0.1

x = 0.0
y = 0.0


if not glfw.init():
    raise Exception("Failed to initialize GLFW")


window = glfw.create_window(
    WIDTH,
    HEIGHT,
    "03 - Moving Object",
    None,
    None
)

if not window:
    glfw.terminate()
    raise Exception("Failed to create window")


glfw.make_context_current(window)


# Previous key states
previous_keys = {
    glfw.KEY_LEFT: glfw.RELEASE,
    glfw.KEY_RIGHT: glfw.RELEASE,
    glfw.KEY_UP: glfw.RELEASE,
    glfw.KEY_DOWN: glfw.RELEASE
}


while not glfw.window_should_close(window):

    # Current key states
    current_keys = {
        glfw.KEY_LEFT: glfw.get_key(window, glfw.KEY_LEFT),
        glfw.KEY_RIGHT: glfw.get_key(window, glfw.KEY_RIGHT),
        glfw.KEY_UP: glfw.get_key(window, glfw.KEY_UP),
        glfw.KEY_DOWN: glfw.get_key(window, glfw.KEY_DOWN)
    }

    # Move only once per key press
    if current_keys[glfw.KEY_LEFT] == glfw.PRESS and \
       previous_keys[glfw.KEY_LEFT] == glfw.RELEASE:
        x -= STEP

    if current_keys[glfw.KEY_RIGHT] == glfw.PRESS and \
       previous_keys[glfw.KEY_RIGHT] == glfw.RELEASE:
        x += STEP

    if current_keys[glfw.KEY_UP] == glfw.PRESS and \
       previous_keys[glfw.KEY_UP] == glfw.RELEASE:
        y += STEP

    if current_keys[glfw.KEY_DOWN] == glfw.PRESS and \
       previous_keys[glfw.KEY_DOWN] == glfw.RELEASE:
        y -= STEP

    # Save current state
    previous_keys = current_keys.copy()

    # Clear screen
    glClearColor(0.05, 0.05, 0.08, 1.0)
    glClear(GL_COLOR_BUFFER_BIT)

    # Object color
    glColor3f(0.0, 1.0, 0.0)

    # Draw rectangle
    glBegin(GL_QUADS)

    glVertex2f(x - 0.1, y - 0.1)
    glVertex2f(x + 0.1, y - 0.1)
    glVertex2f(x + 0.1, y + 0.1)
    glVertex2f(x - 0.1, y + 0.1)

    glEnd()

    glfw.swap_buffers(window)
    glfw.poll_events()


glfw.terminate()
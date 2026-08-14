import glfw
from OpenGL.GL import *

WIDTH=800
HEIGHT=600

if not glfw.init():
    raise Exception("failed to initialize GLFW")

window= glfw.create_window(
    WIDTH,
    HEIGHT,
    "01 - Simple Line",
    None,
    None
)

if not window:
    glfw.terminate()
    raise Exception("Failed to create window")

glfw.make_context_current(window)

while not glfw.window_should_close(window):

    glClearColor(0.05,0.05,0.08,1.0)
    glClear(GL_COLOR_BUFFER_BIT)

    glColor3f(0.0,1.0,0.0)

    glBegin(GL_LINES)

    glVertex2f(-0.6,0.0)
    glVertex2f(0.6,0.0)

    glEnd()

    glfw.swap_buffers(window)
    glfw.poll_events()

glfw.terminate()
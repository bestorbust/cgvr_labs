
import ctypes
import math
import sys

from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GL.shaders import compileShader, compileProgram


# ---------------- WINDOW ----------------

WIDTH, HEIGHT = 1000, 700

window_width = WIDTH
window_height = HEIGHT

program = None
vao = None
vbo = None

angle = 0.0
speed = 1.0
paused = False
last_time = 0.0


# ---------------- VERTEX SHADER ----------------

VERTEX_SHADER = """
#version 330 compatibility

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec3 aColor;

out vec3 vertexColor;

uniform float uAngle;

void main()
{
    float c = cos(uAngle);
    float s = sin(uAngle);

    // Rotate each vertex using the angle uniform.
    vec2 rotatedPosition = vec2(
        c * aPosition.x - s * aPosition.y,
        s * aPosition.x + c * aPosition.y
    );

    gl_Position = vec4(rotatedPosition, 0.0, 1.0);

    // Pass the vertex colour to the fragment shader.
    vertexColor = aColor;
}
"""


# ---------------- FRAGMENT SHADER ----------------

FRAGMENT_SHADER = """
#version 330 compatibility

in vec3 vertexColor;

void main()
{
    // Output the interpolated RGB colour.
    gl_FragColor = vec4(vertexColor, 1.0);
}
"""


# ---------------- TRIANGLE DATA ----------------

# Each vertex contains:
# x, y, red, green, blue

vertices = [
     0.00,  0.65, 1.0, 0.0, 0.0,  # Top: Red
    -0.60, -0.50, 0.0, 1.0, 0.0,  # Left: Green
     0.60, -0.50, 0.0, 0.0, 1.0,  # Right: Blue
]


# ---------------- TEXT ----------------

def draw_text(x, y, message, font=GLUT_BITMAP_HELVETICA_18):
    glRasterPos2f(x, y)

    for character in message:
        glutBitmapCharacter(font, ord(character))


# ---------------- INITIALIZE SHADERS ----------------

def init_shaders():
    global program, vao, vbo

    program = compileProgram(
        compileShader(VERTEX_SHADER, GL_VERTEX_SHADER),
        compileShader(FRAGMENT_SHADER, GL_FRAGMENT_SHADER)
    )

    data = (GLfloat * len(vertices))(*vertices)

    vao = glGenVertexArrays(1)
    vbo = glGenBuffers(1)

    glBindVertexArray(vao)
    glBindBuffer(GL_ARRAY_BUFFER, vbo)

    glBufferData(
        GL_ARRAY_BUFFER,
        ctypes.sizeof(data),
        data,
        GL_STATIC_DRAW
    )

    stride = 5 * ctypes.sizeof(GLfloat)

    # Attribute 0: vertex position (x, y)
    glVertexAttribPointer(
        0, 2, GL_FLOAT, GL_FALSE,
        stride, ctypes.c_void_p(0)
    )
    glEnableVertexAttribArray(0)

    # Attribute 1: vertex colour (r, g, b)
    glVertexAttribPointer(
        1, 3, GL_FLOAT, GL_FALSE,
        stride, ctypes.c_void_p(2 * ctypes.sizeof(GLfloat))
    )
    glEnableVertexAttribArray(1)

    glBindBuffer(GL_ARRAY_BUFFER, 0)
    glBindVertexArray(0)


# ---------------- RESIZE WINDOW ----------------

def reshape(width, height):
    global window_width, window_height

    if width <= 0 or height <= 0:
        return

    window_width = width
    window_height = height

    glViewport(0, 0, width, height)

    glutPostRedisplay()


# ---------------- DRAW INFORMATION ----------------

def draw_overlay():
    glUseProgram(0)

    glViewport(0, 0, window_width, window_height)

    glMatrixMode(GL_PROJECTION)
    glPushMatrix()
    glLoadIdentity()
    glOrtho(0, window_width, 0, window_height, -1, 1)

    glMatrixMode(GL_MODELVIEW)
    glPushMatrix()
    glLoadIdentity()

    glDisable(GL_DEPTH_TEST)
    glColor3f(0.95, 0.96, 1.0)

    # Keep the labels near the top of the window.
    draw_text(
        20, window_height - 35,
        "LAB 8: VERTEX AND FRAGMENT SHADERS"
    )

    draw_text(
        20, window_height - 65,
        "VERTEX SHADER: rotates triangle vertices using uAngle",
        GLUT_BITMAP_HELVETICA_12
    )

    draw_text(
        20, window_height - 85,
        "FRAGMENT SHADER: outputs interpolated RGB colours",
        GLUT_BITMAP_HELVETICA_12
    )

    draw_text(
        20, 55,
        f"Angle: {math.degrees(angle) % 360:.1f} degrees"
        f"   |   Speed: {speed:.1f} rad/s"
    )

    draw_text(
        20, 30,
        "SPACE: Pause/Resume    R: Reset    LEFT/RIGHT: Rotate"
        "    UP/DOWN: Speed    ESC: Exit",
        GLUT_BITMAP_HELVETICA_12
    )

    if paused:
        draw_text(
            window_width // 2 - 45,
            20,
            "PAUSED",
            GLUT_BITMAP_HELVETICA_18
        )

    glPopMatrix()

    glMatrixMode(GL_PROJECTION)
    glPopMatrix()

    glMatrixMode(GL_MODELVIEW)

    glEnable(GL_DEPTH_TEST)


# ---------------- DISPLAY ----------------

def display():
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    glUseProgram(program)

    angle_location = glGetUniformLocation(program, "uAngle")
    glUniform1f(angle_location, angle)

    glBindVertexArray(vao)
    glDrawArrays(GL_TRIANGLES, 0, 3)
    glBindVertexArray(0)

    draw_overlay()

    glutSwapBuffers()


# ---------------- ANIMATION ----------------

def update():
    global angle, last_time

    current_time = glutGet(GLUT_ELAPSED_TIME) / 1000.0
    delta_time = current_time - last_time
    last_time = current_time

    # Avoid a large jump after a pause or window restore.
    delta_time = min(delta_time, 0.05)

    if not paused:
        angle += speed * delta_time

    glutPostRedisplay()


# ---------------- KEYBOARD CONTROLS ----------------

def keyboard(key, x, y):
    global angle, speed, paused

    if key == b'\x1b':  # Escape
        try:
            glDeleteBuffers(1, [vbo])
            glDeleteVertexArrays(1, [vao])
            glDeleteProgram(program)
        except Exception:
            pass

        sys.exit(0)

    elif key == b' ':
        paused = not paused

    elif key in (b'r', b'R'):
        angle = 0.0
        paused = False

    elif key in (b'+', b'='):
        speed = min(3.0, speed + 0.25)

    elif key in (b'-', b'_'):
        speed = max(0.0, speed - 0.25)

    glutPostRedisplay()


def special_keys(key, x, y):
    global angle, speed, paused

    if key == GLUT_KEY_LEFT:
        angle -= math.radians(15)
        paused = True

    elif key == GLUT_KEY_RIGHT:
        angle += math.radians(15)
        paused = True

    elif key == GLUT_KEY_UP:
        speed = min(3.0, speed + 0.25)

    elif key == GLUT_KEY_DOWN:
        speed = max(0.0, speed - 0.25)

    glutPostRedisplay()


# ---------------- INITIALIZATION ----------------

def init():
    glClearColor(0.035, 0.045, 0.075, 1.0)
    glDisable(GL_DEPTH_TEST)

    init_shaders()


# ---------------- MAIN ----------------

def main():
    global last_time

    glutInit()
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH)
    glutInitWindowSize(WIDTH, HEIGHT)
    glutInitWindowPosition(100, 50)

    # Request an OpenGL 3.3 compatibility context.
    glutInitContextVersion(3, 3)
    glutInitContextProfile(GLUT_COMPATIBILITY_PROFILE)

    glutCreateWindow(b"Lab 8 - Vertex and Fragment Shaders")

    init()

    last_time = glutGet(GLUT_ELAPSED_TIME) / 1000.0

    glutDisplayFunc(display)
    glutReshapeFunc(reshape)
    glutKeyboardFunc(keyboard)
    glutSpecialFunc(special_keys)
    glutIdleFunc(update)

    glutMainLoop()


if __name__ == "__main__":
    main()

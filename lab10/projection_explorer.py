"""Lab 10: Orthographic vs perspective projection, PyOpenGL + GLUT.
Demonstrates the 3D viewing pipeline comparing parallel rays vs. foreshortening side-by-side.
"""
import math
import sys
from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *

W, H = 1100, 700
width, height = W, H
angle = 25.0
fov = 45.0
distance = 8.0
running = True
wireframe = False
last_ms = 0

# Each face is defined with an outward-facing normal and its own colour.
FACES = [
    ((0, 0, 1), (0.15, 0.78, 0.90), [(-1, -1, 1), (1, -1, 1), (1, 1, 1), (-1, 1, 1)]),
    ((0, 0, -1), (0.75, 0.35, 0.90), [(1, -1, -1), (-1, -1, -1), (-1, 1, -1), (1, 1, -1)]),
    ((1, 0, 0), (0.98, 0.48, 0.33), [(1, -1, 1), (1, -1, -1), (1, 1, -1), (1, 1, 1)]),
    ((-1, 0, 0), (0.28, 0.76, 0.47), [(-1, -1, -1), (-1, -1, 1), (-1, 1, 1), (-1, 1, -1)]),
    ((0, 1, 0), (1.00, 0.82, 0.32), [(-1, 1, 1), (1, 1, 1), (1, 1, -1), (-1, 1, -1)]),
    ((0, -1, 0), (0.40, 0.54, 0.98), [(-1, -1, -1), (1, -1, -1), (1, -1, 1), (-1, -1, 1)]),
]


def cube():
    for normal, color, points in FACES:
        glColor3f(*color)
        glBegin(GL_QUADS)
        glNormal3f(*normal)
        for p in points:
            glVertex3f(*p)
        glEnd()
    glColor3f(0.08, 0.10, 0.16)
    glLineWidth(2.0)
    for _, _, points in FACES:
        glBegin(GL_LINE_LOOP)
        for p in points:
            glVertex3f(*p)
        glEnd()


def draw_scene(x, viewport_width, perspective):
    if viewport_width < 1 or height < 1:
        return
    # Reserve space at the top for labels and bottom for controls.
    bottom = max(62, int(height * 0.10))
    top = max(110, int(height * 0.18))
    scene_h = max(1, height - bottom - top)
    glViewport(x, bottom, viewport_width, scene_h)
    aspect = viewport_width / float(scene_h)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    if perspective:
        gluPerspective(fov, aspect, 0.1, 100.0)
    else:
        span = 3.5
        if aspect >= 1:
            glOrtho(-span * aspect, span * aspect, -span, span, 0.1, 100)
        else:
            glOrtho(-span, span, -span / aspect, span / aspect, 0.1, 100)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()
    gluLookAt(distance * 0.55, distance * 0.38, distance * 0.75,
              0, 0, 0, 0, 1, 0)
    glRotatef(angle, 0.4, 1, 0.2)
    if wireframe:
        glPolygonMode(GL_FRONT_AND_BACK, GL_LINE)
    cube()
    glPolygonMode(GL_FRONT_AND_BACK, GL_FILL)


def text(x, y, message, font=GLUT_BITMAP_HELVETICA_12):
    glRasterPos2f(x, y)
    for char in message:
        glutBitmapCharacter(font, ord(char))


def overlay():
    glViewport(0, 0, width, height)
    glMatrixMode(GL_PROJECTION)
    glPushMatrix()
    glLoadIdentity()
    glOrtho(0, width, 0, height, -1, 1)
    glMatrixMode(GL_MODELVIEW)
    glPushMatrix()
    glLoadIdentity()
    glDisable(GL_DEPTH_TEST)
    glColor3f(0.94, 0.96, 1)

    half = width // 2
    text(16, height - 28, 'LAB 10  |  3D VIEWING PIPELINE', GLUT_BITMAP_HELVETICA_18)
    text(16, height - 60, 'LEFT: ORTHOGRAPHIC', GLUT_BITMAP_HELVETICA_18)
    text(half + 16, height - 60, 'RIGHT: PERSPECTIVE', GLUT_BITMAP_HELVETICA_18)
    text(16, height - 82, 'Parallel rays; size independent of distance')
    text(half + 16, height - 82, 'Converging rays; distant objects appear smaller')

    # Status & Controls bar at the bottom
    status = f"Space: {'Pause' if running else 'Play'}   R: Reset   W: Wireframe ({'ON' if wireframe else 'OFF'})   FOV: {fov:.0f}°   Angle: {angle:.0f}°   Esc: Quit"
    text(16, 24, status)

    # Dividing separator line
    glColor3f(0.25, 0.44, 0.65)
    glLineWidth(2.0)
    glBegin(GL_LINES)
    glVertex2f(half, bottom_offset := max(62, int(height * 0.10)))
    glVertex2f(half, height - max(110, int(height * 0.18)))
    glEnd()

    glEnable(GL_DEPTH_TEST)
    glMatrixMode(GL_MODELVIEW)
    glPopMatrix()
    glMatrixMode(GL_PROJECTION)
    glPopMatrix()


def display():
    glClearColor(0.045, 0.065, 0.10, 1.0)
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    half = width // 2
    # Left: Orthographic Projection
    draw_scene(0, half, perspective=False)
    # Right: Perspective Projection
    draw_scene(half, width - half, perspective=True)

    overlay()
    glutSwapBuffers()


def reshape(w, h):
    global width, height
    width = max(1, w)
    height = max(1, h)


def keyboard(key, x, y):
    global running, angle, wireframe, fov
    if key == b' ':
        running = not running
    elif key in (b'r', b'R'):
        angle = 25.0
        fov = 45.0
        running = True
    elif key in (b'w', b'W'):
        wireframe = not wireframe
    elif key in (b'+', b'='):
        fov = min(75.0, fov + 5.0)
    elif key in (b'-', b'_'):
        fov = max(20.0, fov - 5.0)
    elif key == b'\x1b':  # Esc key
        sys.exit(0)
    glutPostRedisplay()


def idle():
    global angle, last_ms
    current_ms = glutGet(GLUT_ELAPSED_TIME)
    if last_ms == 0:
        last_ms = current_ms
    dt = (current_ms - last_ms) / 1000.0
    last_ms = current_ms

    if running:
        angle = (angle + 30.0 * dt) % 360.0
        glutPostRedisplay()


def main():
    glutInit(sys.argv)
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH)
    glutInitWindowSize(W, H)
    glutInitWindowPosition(100, 100)
    glutCreateWindow(b"Lab 10 - 3D Viewing Pipeline & Projection Explorer")

    glEnable(GL_DEPTH_TEST)
    glDepthFunc(GL_LEQUAL)
    glShadeModel(GL_SMOOTH)

    glutDisplayFunc(display)
    glutReshapeFunc(reshape)
    glutKeyboardFunc(keyboard)
    glutIdleFunc(idle)

    glutMainLoop()


if __name__ == '__main__':
    main()

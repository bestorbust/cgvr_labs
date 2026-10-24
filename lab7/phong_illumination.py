
from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

WIDTH, HEIGHT = 1100, 720

window_width = WIDTH
window_height = HEIGHT
quadric = None


# ---------------- MATERIAL ----------------

def set_material():
    glMaterialfv(GL_FRONT, GL_AMBIENT, [0.8, 0.12, 0.08, 1.0])
    glMaterialfv(GL_FRONT, GL_DIFFUSE, [0.9, 0.15, 0.1, 1.0])
    glMaterialfv(GL_FRONT, GL_SPECULAR, [1.0, 1.0, 1.0, 1.0])
    glMaterialf(GL_FRONT, GL_SHININESS, 64.0)


# ---------------- LIGHT ----------------

def set_light(ambient, diffuse, specular):
    glLightfv(GL_LIGHT0, GL_POSITION, [3.0, 4.0, 5.0, 1.0])
    glLightfv(GL_LIGHT0, GL_AMBIENT, [ambient, ambient, ambient, 1.0])
    glLightfv(GL_LIGHT0, GL_DIFFUSE, [diffuse, diffuse, diffuse, 1.0])
    glLightfv(GL_LIGHT0, GL_SPECULAR, [specular, specular, specular, 1.0])


# ---------------- WINDOW RESIZE ----------------

def reshape(width, height):
    global window_width, window_height

    # Avoid zero-sized viewports when minimizing.
    if width <= 0 or height <= 0:
        return

    window_width = width
    window_height = height

    glViewport(0, 0, width, height)
    glutPostRedisplay()


# ---------------- TEXT ----------------

def draw_text(x, y, text, font=GLUT_BITMAP_HELVETICA_18):
    glRasterPos2f(x, y)

    for ch in text:
        glutBitmapCharacter(font, ord(ch))


# ---------------- DRAW ONE SPHERE ----------------

def draw_sphere_panel(x, y, width, height, ambient, diffuse, specular):
    if width <= 0 or height <= 0:
        return

    # Render the sphere inside its own quadrant.
    glViewport(x, y, width, height)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluPerspective(40.0, width / float(height), 0.1, 100.0)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()

    gluLookAt(
        0.0, 0.0, 6.0,
        0.0, 0.0, 0.0,
        0.0, 1.0, 0.0
    )

    # Light position is set after the camera.
    set_light(ambient, diffuse, specular)
    set_material()

    glPushMatrix()
    glRotatef(20.0, 0.0, 1.0, 0.0)
    gluSphere(quadric, 1.15, 64, 64)
    glPopMatrix()


# ---------------- DRAW LABELS AND DIVIDERS ----------------

def draw_overlay():
    half_w = window_width // 2
    half_h = window_height // 2

    # Use the full window for all text and divider lines.
    glViewport(0, 0, window_width, window_height)

    glMatrixMode(GL_PROJECTION)
    glPushMatrix()
    glLoadIdentity()
    gluOrtho2D(0, window_width, 0, window_height)

    glMatrixMode(GL_MODELVIEW)
    glPushMatrix()
    glLoadIdentity()

    glDisable(GL_LIGHTING)
    glDisable(GL_DEPTH_TEST)

    glColor3f(0.92, 0.95, 1.0)

    # Use smaller title text when the window is narrow.
    title_font = (
        GLUT_BITMAP_HELVETICA_18
        if window_width >= 850
        else GLUT_BITMAP_HELVETICA_12
    )

    # Top-left: Ambient
    draw_text(15, window_height - 30,
              "1. AMBIENT ILLUMINATION", title_font)
    draw_text(15, window_height - 52,
              "Base illumination across the surface",
              GLUT_BITMAP_HELVETICA_12)

    # Top-right: Diffuse
    draw_text(half_w + 15, window_height - 30,
              "2. DIFFUSE ILLUMINATION", title_font)
    draw_text(half_w + 15, window_height - 52,
              "Brightness depends on light-surface angle",
              GLUT_BITMAP_HELVETICA_12)

    # Bottom-left: Specular
    draw_text(15, half_h - 30,
              "3. SPECULAR ILLUMINATION", title_font)
    draw_text(15, half_h - 52,
              "White highlight; shininess = 64",
              GLUT_BITMAP_HELVETICA_12)

    # Bottom-right: Combined Phong
    draw_text(half_w + 15, half_h - 30,
              "4. PHONG: COMBINED LIGHTING", title_font)
    draw_text(half_w + 15, half_h - 52,
              "Ambient + Diffuse + Specular",
              GLUT_BITMAP_HELVETICA_12)

    # Draw quadrant divider lines.
    glColor3f(0.3, 0.35, 0.45)
    glLineWidth(2.0)

    glBegin(GL_LINES)
    glVertex2f(0, half_h)
    glVertex2f(window_width, half_h)

    glVertex2f(half_w, 0)
    glVertex2f(half_w, window_height)
    glEnd()

    # Restore matrices and rendering state.
    glPopMatrix()

    glMatrixMode(GL_PROJECTION)
    glPopMatrix()

    glMatrixMode(GL_MODELVIEW)

    glEnable(GL_DEPTH_TEST)
    glEnable(GL_LIGHTING)


# ---------------- DISPLAY ----------------

def display():
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)

    half_w = window_width // 2
    half_h = window_height // 2

    # 1. Ambient only
    draw_sphere_panel(
        0, half_h, half_w, window_height - half_h,
        1.0, 0.0, 0.0
    )

    # 2. Diffuse only
    draw_sphere_panel(
        half_w, half_h,
        window_width - half_w, window_height - half_h,
        0.0, 1.0, 0.0
    )

    # 3. Specular only
    draw_sphere_panel(
        0, 0, half_w, half_h,
        0.0, 0.0, 1.0
    )

    # 4. Combined Phong illumination
    draw_sphere_panel(
        half_w, 0,
        window_width - half_w, half_h,
        0.25, 1.0, 1.0
    )

    # Draw text after the spheres so it stays in the correct position.
    draw_overlay()

    glutSwapBuffers()


# ---------------- INITIALIZATION ----------------

def init():
    glClearColor(0.035, 0.045, 0.075, 1.0)

    glEnable(GL_DEPTH_TEST)
    glDepthFunc(GL_LESS)

    glEnable(GL_LIGHTING)
    glEnable(GL_LIGHT0)
    glEnable(GL_NORMALIZE)

    # Remove OpenGL's default global ambient contribution.
    glLightModelfv(
        GL_LIGHT_MODEL_AMBIENT,
        [0.0, 0.0, 0.0, 1.0]
    )

    glShadeModel(GL_SMOOTH)
    glLightModeli(GL_LIGHT_MODEL_LOCAL_VIEWER, GL_TRUE)


# ---------------- MAIN ----------------

def main():
    global quadric

    glutInit()
    glutInitDisplayMode(
        GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH
    )
    glutInitWindowSize(WIDTH, HEIGHT)
    glutInitWindowPosition(100, 50)

    glutCreateWindow(b"Lab 7 - Phong Illumination Model")

    init()

    quadric = gluNewQuadric()
    gluQuadricNormals(quadric, GLU_SMOOTH)

    glutDisplayFunc(display)
    glutReshapeFunc(reshape)

    glutMainLoop()


if __name__ == "__main__":
    main()

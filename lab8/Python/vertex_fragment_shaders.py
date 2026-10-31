"""Lab 8: Vertex and Fragment Shaders. Python + PyOpenGL + FreeGLUT."""
import ctypes
import math
from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GL.shaders import compileShader, compileProgram

W, H = 1000, 700
width, height = W, H
angle, speed, paused, last_ms = 0.0, 1.0, False, 0
program = vao = vbo = None

VERTEX = """#version 330 compatibility
layout(location=0) in vec2 position;
layout(location=1) in vec3 color;
out vec3 rgb;
uniform float rotation;
uniform float aspect;
void main() {
    float c=cos(rotation), s=sin(rotation);
    vec2 p=vec2(c*position.x-s*position.y, s*position.x+c*position.y);
    p.x /= aspect;
    gl_Position=vec4(p,0.0,1.0);
    rgb=color;
}
"""
FRAGMENT = """#version 330 compatibility
in vec3 rgb;
void main() { gl_FragColor=vec4(rgb,1.0); }
"""

def text(x,y,msg,font=GLUT_BITMAP_HELVETICA_12):
    glRasterPos2f(x,y)
    for ch in msg: glutBitmapCharacter(font,ord(ch))

def init():
    global program,vao,vbo
    glClearColor(.035,.045,.075,1)
    program=compileProgram(compileShader(VERTEX,GL_VERTEX_SHADER),compileShader(FRAGMENT,GL_FRAGMENT_SHADER))
    data=(GLfloat*15)(0,.68,1,0,0, -.65,-.5,0,1,0, .65,-.5,0,0,1)
    vao=glGenVertexArrays(1); vbo=glGenBuffers(1)
    glBindVertexArray(vao); glBindBuffer(GL_ARRAY_BUFFER,vbo)
    glBufferData(GL_ARRAY_BUFFER,ctypes.sizeof(data),data,GL_STATIC_DRAW)
    stride=5*ctypes.sizeof(GLfloat)
    glVertexAttribPointer(0,2,GL_FLOAT,GL_FALSE,stride,ctypes.c_void_p(0))
    glEnableVertexAttribArray(0)
    glVertexAttribPointer(1,3,GL_FLOAT,GL_FALSE,stride,ctypes.c_void_p(2*ctypes.sizeof(GLfloat)))
    glEnableVertexAttribArray(1)
    glBindVertexArray(0)

def reshape(w,h):
    global width,height
    if w<=0 or h<=0:return
    width,height=w,h
    glViewport(0,0,w,h)
    glutPostRedisplay()

def overlay():
    glUseProgram(0)
    glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity()
    gluOrtho2D(0,width,0,height)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity()
    glColor3f(.95,.96,1)
    text(18,height-32,'LAB 8 | VERTEX + FRAGMENT SHADERS',GLUT_BITMAP_HELVETICA_18)
    text(18,height-57,'Vertex shader: rotates positions | Fragment shader: interpolates RGB')
    text(18,48,f'Angle: {math.degrees(angle)%360:.0f} deg   Speed: {speed:.2f} rad/s   {"PAUSED" if paused else "RUNNING"}')
    text(18,27,'SPACE pause   R reset   LEFT/RIGHT rotate   UP/DOWN speed   ESC exit')
    glPopMatrix(); glMatrixMode(GL_PROJECTION);glPopMatrix();glMatrixMode(GL_MODELVIEW)

def display():
    glClear(GL_COLOR_BUFFER_BIT)
    glUseProgram(program)
    glUniform1f(glGetUniformLocation(program,'rotation'),angle)
    glUniform1f(glGetUniformLocation(program,'aspect'),width/float(height))
    glBindVertexArray(vao);glDrawArrays(GL_TRIANGLES,0,3);glBindVertexArray(0)
    overlay()
    glutSwapBuffers()

def idle():
    global angle,last_ms
    now=glutGet(GLUT_ELAPSED_TIME)
    dt=min(max(0,now-last_ms)/1000.,.05)
    last_ms=now
    if not paused:angle=(angle+dt*speed)%(2*math.pi)
    glutPostRedisplay()

def keyboard(key,x,y):
    global paused,angle
    if key==b'\x1b':
        glutLeaveMainLoop()
    elif key==b' ':paused=not paused
    elif key.lower()==b'r':angle=0.;paused=False
    glutPostRedisplay()

def special(key,x,y):
    global paused,angle,speed
    if key==GLUT_KEY_LEFT:angle-=math.radians(15);paused=True
    elif key==GLUT_KEY_RIGHT:angle+=math.radians(15);paused=True
    elif key==GLUT_KEY_UP:speed=min(4,speed+.25)
    elif key==GLUT_KEY_DOWN:speed=max(0,speed-.25)
    glutPostRedisplay()

def main():
    global last_ms
    glutInit()
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGBA)
    glutInitWindowSize(W,H)
    # GLSL 3.30 + legacy bitmap text overlay needs compatibility profile.
    glutInitContextVersion(3,3)
    glutInitContextProfile(GLUT_COMPATIBILITY_PROFILE)
    glutCreateWindow(b'Lab 8 - RGB Shader Triangle')
    init()
    last_ms=glutGet(GLUT_ELAPSED_TIME)
    glutDisplayFunc(display);glutReshapeFunc(reshape)
    glutKeyboardFunc(keyboard);glutSpecialFunc(special);glutIdleFunc(idle)
    glutMainLoop()

if __name__=='__main__':main()

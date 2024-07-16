function [canvas,minX,minY]=PlotLines(slices,cutPlane)
L=size(slices{1}.array,2);

X=0;
Y=0;
cc=0;
for I=1:length(slices)
    if isfield(slices{I},'Point1')==true && slices{I}.AlreadyDone==true
        [x y]=findPoint(slices{I}, [1 L]);
        xx=x(1)+x(2);
        yy=y(1)+y(2);
        if isnan(xx)==false && isinf(xx)==false && isnan(yy)==false && isinf(yy)==false
            X=X+x(1)+x(2);
            Y=Y+y(1)+y(2);
            cc=cc+2;
        end
    end
end
X=X/cc;
Y=Y/cc;

minX=X-L/2;
minY=Y-L/2;
maxX=X+L/2;
maxY=Y+L/2;

canvas = zeros([maxX-minX maxY-minY]);
count =zeros(size(canvas));
for I=1:length(slices)
    if isfield(slices{I},'Point1')==true && slices{I}.AlreadyDone==true
        A1=slices{I}.array(:,cutPlane);
        A1(A1<0)=0;
        [x y]=findPoint(slices{I}, 1:L);
        for J=1:L
            i=round(x(J)-minX);
            j=round(y(J)-minY);
            if (isnan(i)==false && isnan(j)==false && isinf(i)==false && isinf(j)==false)
                if (i>1 && j>1 && i<L &&j<L)
                    canvas(i,j)=canvas(i,j) + A1(J);
                    count(i,j)=count(i,j)+1;
                end
            end
        end
    end
end

idx=find(count>0);
canvas(idx)=canvas(idx)./count(idx);

sdCanvas=zeros(size(canvas));
for I=1:length(slices)
    if isfield(slices{I},'Point1')==true && slices{I}.AlreadyDone==true
        A1=slices{I}.array(:,cutPlane);
        A1(A1<0)=0;
        [x y]=findPoint(slices{I}, 1:L);
        for J=1:L
            i=round(x(J)-minX);
            j=round(y(J)-minY);
            if (isnan(i)==false && isnan(j)==false && isinf(i)==false && isinf(j)==false)
                if (i>1 && j>1 && i<L &&j<L)
                    sdCanvas(i,j)= (canvas(i,j) - A1(J))^2;
                end
            end
        end
    end
end

sdCanvas(idx)=(sdCanvas(idx)./count(idx)).^(.5)*4;


canvas2 =zeros(size(canvas));
count2 =zeros(size(canvas));
for I=1:length(slices)
    if isfield(slices{I},'Point1')==true && slices{I}.AlreadyDone==true
        A1=slices{I}.array(:,cutPlane);
        A1(A1<0)=0;
        [x y]=findPoint(slices{I}, 1:L);
        for J=1:L
            i=round(x(J)-minX);
            j=round(y(J)-minY);
            if (isnan(i)==false && isnan(j)==false && isinf(i)==false && isinf(j)==false)
                if (i>1 && j>1 && i<L &&j<L)
                    if abs(canvas(i,j)-A1(J))<sdCanvas(i,j)
                        canvas2(i,j)=canvas2(i,j) + A1(J);
                        count2(i,j)=count2(i,j)+1;
                    end
                end
            end
        end
    end
end

canvas2(idx)=canvas2(idx)./count(idx);
%canvas =canvas(L/2:end-L/2,L/2:end-L/2);

end


